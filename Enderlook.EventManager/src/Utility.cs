using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal static class Utility
    {
        private const int INITIAL_CAPACITY = 4;
        private const int GROW_FACTOR = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Steal<T>(ref T value) where T : class
        {
            T value_;
            do
            {
                value_ = Interlocked.Exchange(ref value, null);
            } while (value_ is null);
            return value_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InnerAdd<T>(ref T[] array, ref int count, T element)
        {
            T[] array_ = Steal(ref array);

            try
            {
                int count_ = count;
                if (count_ == array.Length)
                {
                    if (count_ == 0)
                        array_ = ArrayPool<T>.Shared.Rent(INITIAL_CAPACITY);
                    else
                    {
                        T[] newArray = ArrayPool<T>.Shared.Rent(count_ * GROW_FACTOR);
                        Array.Copy(array_, newArray, count_);
                        ArrayPool<T>.Shared.Return(array_);
                        array_ = newArray;
                    }
                }
                array_[count_++] = element;
                count = count_;
            }
            finally
            {
                array = array_;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InnerSwap<T>(ref T[] array, ref int count, ref T[] newArray, out int newCount)
        {
            T[] array_ = Steal(ref array);

            newCount = count;
            count = 0;
            array = newArray;
            newArray = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractToRun<T>(
            ref T[] toRun, ref int toRunCount, ref T[] toRemove, ref int toRemoveCount,
            ref T[] toRunExtracted, out int toRunExtractedCount, ref T[] removedArray, out int removedArrayCount)
        {
            ref T[] toRemoveExtracted = ref removedArray;

            InnerSwap(ref toRun, ref toRunCount, ref toRunExtracted, out toRunExtractedCount);
            InnerSwap(ref toRemove, ref toRemoveCount, ref toRemoveExtracted, out int countRemove);
            if (countRemove > 0)
            {
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                T _ = toRunExtracted[toRunExtractedCount];
                _ = toRemoveExtracted[countRemove];
                for (int i = 0; i < toRunExtractedCount; i++)
                {
                    T element = toRunExtracted[i];
                    for (int k = countRemove - 1; k >= 0; k--)
                    {
                        if (element.Equals(toRemoveExtracted[k]))
                        {
                            Array.Copy(toRemoveExtracted, k + 1, toRemoveExtracted, k, countRemove - k);
                            countRemove--;
                            goto next;
                        }
                        toRunExtracted[j++] = element;
                        next:;
                    }
                }
                toRunExtractedCount = j;
            }

            removedArrayCount = countRemove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Drain<T>(ref T[] destination, ref int destiantionCount, ref T[] source, ref int sourceCount)
        {
            T[] array_;
            do
            {
                array_ = Interlocked.Exchange(ref destination, null);
            } while (array_ is null);

            T[] from;
            T[] to;
            int fromCount;
            int toCount;
            if (source.Length > array_.Length)
            {
                to = source;
                toCount = sourceCount;
                from = array_;
                fromCount = destiantionCount;
            }
            else
            {
                from = source;
                fromCount = sourceCount;
                to = array_;
                toCount = destiantionCount;
            }

            int totalCount = fromCount + toCount;
            if (totalCount < to.Length)
            {
                Array.Copy(from, 0, to, toCount + 1, fromCount);
                source = from;
                sourceCount = fromCount;
                destiantionCount = totalCount;
                destination = to;
            }
            else
            {
                T[] newArray = ArrayPool<T>.Shared.Rent(totalCount * GROW_FACTOR);
                Array.Copy(from, newArray, fromCount);
                Array.Copy(to, 0, newArray, fromCount + 1, toCount);
                source = to;
                sourceCount = toCount;
                ArrayPool<T>.Shared.Return(from);
                destiantionCount = totalCount;
                destination = newArray;
            }
        }

        public static void InnerRaise<TEvent, TDelegate>(ref TDelegate[] a, int count, TEvent argument)
        {
            TDelegate _ = a[count];
            for (int i = 0; i < count; i++)
            {
                if (typeof(TEvent) == typeof(Parameterless))
                {
                    Debug.Assert(a[i] is Action);
                    Unsafe.As<Action>(a[i])();
                }
                else
                {
                    Debug.Assert(a[i] is Action<TEvent>);
                    Unsafe.As<Action<TEvent>>(a[i])(argument);
                }
            }
        }

        public static void InnerRaise<TEvent, TDelegate>(ref TDelegate[] a, ref TDelegate[] b, int count, int countRemove, TEvent argument)
        {
            TDelegate _ = a[count];
            _ = b[countRemove];
            for (int i = 0; i < count; i++)
            {
                TDelegate element = a[i];
                for (int j = countRemove - 1; j >= 0; j--)
                {
                    if (element.Equals(b[j]))
                    {
                        Array.Copy(b, j + 1, b, j, countRemove - j);
                        countRemove--;
                        goto next;
                    }
                }

                if (typeof(TEvent) == typeof(Parameterless))
                {
                    Debug.Assert(element is Action);
                    Unsafe.As<Action>(element)();
                }
                else
                {
                    Debug.Assert(element is Action<TEvent>);
                    Unsafe.As<Action<TEvent>>(element)(argument);
                }

                next:;
            }
        }

        public static class Container<T>
        {
            public static readonly T[] empty = new T[0];
            public static T[] array1 = empty;
            public static T[] array2 = empty;
            public static T[] array3 = empty;
            public static T[] array4 = empty;
        }

        public static void Raise<TEvent, TParameterless, TParameters>(
            ref EventList<TParameterless> parameterless,
            ref EventList<TParameters> parameters,
            ref EventListOnce<TParameterless> parameterlessOnce,
            ref EventListOnce<TParameters> parametersOnce,
            TEvent argument)
        {
            TParameterless[] parameterless1 = Interlocked.Exchange(ref Container<TParameterless>.array1, Container<TParameterless>.empty);
            TParameterless[] parameterless2 = Interlocked.Exchange(ref Container<TParameterless>.array2, Container<TParameterless>.empty);
            TParameterless[] parameterlessOnce1 = Interlocked.Exchange(ref Container<TParameterless>.array3, Container<TParameterless>.empty);
            TParameterless[] parameterlessOnce2 = Interlocked.Exchange(ref Container<TParameterless>.array4, Container<TParameterless>.empty);
            TParameters[] parameters1 = Interlocked.Exchange(ref Container<TParameters>.array1, Container<TParameters>.empty);
            TParameters[] parameters2 = Interlocked.Exchange(ref Container<TParameters>.array2, Container<TParameters>.empty);
            TParameters[] parametersOnce1 = Interlocked.Exchange(ref Container<TParameters>.array3, Container<TParameters>.empty);
            TParameters[] parametersOnce2 = Interlocked.Exchange(ref Container<TParameters>.array4, Container<TParameters>.empty);

            parameterless.ExtractToRun(ref parameterless1, out int parameterlessCount1, ref parameterless2, out int parameterlessCount2);
            parameterlessOnce.ExtractToRun(ref parameterlessOnce1, out int parameterlessOnceCount1, ref parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(ref parameters1, out int parametersCount1, ref parameters2, out int parametersCount2);
            parametersOnce.ExtractToRun(ref parametersOnce1, out int parametersOnceCount1, ref parametersOnce2, out int parametersOnceCount2);

            InnerRaise(ref parameterless1, parameterlessCount1, new Parameterless());
            InnerRaise(ref parameterlessOnce1, parametersCount1, new Parameterless());
            InnerRaise(ref parameters1, parametersCount1, argument);
            InnerRaise(ref parametersOnce1, parametersOnceCount1, argument);

            parameterless.InjectToRun(ref parameterless1, ref parameterlessCount1);
            parameterlessOnce.InjectToRun(ref parameterlessOnce1, ref parameterlessOnceCount1);
            parameters.InjectToRun(ref parameters1, ref parametersCount1);
            parametersOnce.InjectToRun(ref parametersOnce1, ref parametersOnceCount1);

            Cleaning(parameterless1, parameterless2, parameterlessOnce1, parameterlessOnce2,
                     parameters1, parameters2, parametersOnce1, parametersOnce2,
                     parameterlessCount1, parameterlessCount2, parameterlessOnceCount1, parameterlessOnceCount2,
                     parametersCount1, parametersCount2, parametersOnceCount1, parametersOnceCount2);
        }

        public static void Purge<TParameterless, TParameters>(
            ref EventList<TParameterless> parameterless,
            ref EventList<TParameters> parameters,
            ref EventListOnce<TParameterless> parameterlessOnce,
            ref EventListOnce<TParameters> parametersOnce)
        {
            TParameterless[] parameterless1 = Interlocked.Exchange(ref Container<TParameterless>.array1, Container<TParameterless>.empty);
            TParameterless[] parameterless2 = Interlocked.Exchange(ref Container<TParameterless>.array2, Container<TParameterless>.empty);
            TParameterless[] parameterlessOnce1 = Interlocked.Exchange(ref Container<TParameterless>.array3, Container<TParameterless>.empty);
            TParameterless[] parameterlessOnce2 = Interlocked.Exchange(ref Container<TParameterless>.array4, Container<TParameterless>.empty);
            TParameters[] parameters1 = Interlocked.Exchange(ref Container<TParameters>.array1, Container<TParameters>.empty);
            TParameters[] parameters2 = Interlocked.Exchange(ref Container<TParameters>.array2, Container<TParameters>.empty);
            TParameters[] parametersOnce1 = Interlocked.Exchange(ref Container<TParameters>.array3, Container<TParameters>.empty);
            TParameters[] parametersOnce2 = Interlocked.Exchange(ref Container<TParameters>.array4, Container<TParameters>.empty);

            parameterless.ExtractToRun(ref parameterless1, out int parameterlessCount1, ref parameterless2, out int parameterlessCount2);
            parameterless.InjectToRun(ref parameterless1, ref parameterlessCount1);
            parameterlessOnce.ExtractToRun(ref parameterlessOnce1, out int parameterlessOnceCount1, ref parameterlessOnce2, out int parameterlessOnceCount2);
            parameterlessOnce.InjectToRun(ref parameterlessOnce1, ref parameterlessOnceCount1);

            parameters.ExtractToRun(ref parameters1, out int parametersCount1, ref parameters2, out int parametersCount2);
            parameters.InjectToRun(ref parameters1, ref parametersCount1);
            parametersOnce.ExtractToRun(ref parametersOnce1, out int parametersOnceCount1, ref parametersOnce2, out int parametersOnceCount2);
            parametersOnce.InjectToRun(ref parametersOnce1, ref parametersOnceCount1);

            Cleaning(
                parameterless1, parameterless2, parameterlessOnce1, parameterlessOnce2,
                parameters1, parameters2, parametersOnce1, parametersOnce2,
                parameterlessCount1, parameterlessCount2, parameterlessOnceCount1, parameterlessOnceCount2,
                parametersCount1, parametersCount2, parametersOnceCount1, parametersOnceCount2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Cleaning<TParameterless, TParameters>(
            TParameterless[] parameterless1, TParameterless[] parameterless2, TParameterless[] parameterlessOnce1, TParameterless[] parameterlessOnce2,
            TParameters[] parameters1, TParameters[] parameters2, TParameters[] parametersOnce1, TParameters[] parametersOnce2,
            int parameterlessCount1, int parameterlessCount2, int parameterlessOnceCount1, int parameterlessOnceCount2,
            int parametersCount1, int parametersCount2, int parametersOnceCount1, int parametersOnceCount2)
        {
            Array.Clear(parameterless1, 0, parameterlessCount1);
            Array.Clear(parameterless2, 0, parameterlessCount2);
            Array.Clear(parameterlessOnce1, 0, parameterlessOnceCount1);
            Array.Clear(parameterlessOnce2, 0, parameterlessOnceCount2);
            Array.Clear(parameters1, 0, parametersCount1);
            Array.Clear(parameters2, 0, parametersCount2);
            Array.Clear(parametersOnce1, 0, parametersOnceCount1);
            Array.Clear(parametersOnce2, 0, parametersOnceCount2);

            TParameterless[] array = Interlocked.Exchange(ref Container<TParameterless>.array1, parameterless1);
            TParameterless[] array1 = Interlocked.Exchange(ref Container<TParameterless>.array2, parameterless2);
            TParameterless[] array2 = Interlocked.Exchange(ref Container<TParameterless>.array3, parameterlessOnce1);
            TParameterless[] array3 = Interlocked.Exchange(ref Container<TParameterless>.array4, parameterlessOnce2);
            TParameters[] array4 = Interlocked.Exchange(ref Container<TParameters>.array1, parameters1);
            TParameters[] array5 = Interlocked.Exchange(ref Container<TParameters>.array2, parameters2);
            TParameters[] array6 = Interlocked.Exchange(ref Container<TParameters>.array3, parametersOnce1);
            TParameters[] array7 = Interlocked.Exchange(ref Container<TParameters>.array4, parametersOnce2);

            ArrayPool<TParameterless>.Shared.Return(array);
            ArrayPool<TParameterless>.Shared.Return(array1);
            ArrayPool<TParameterless>.Shared.Return(array2);
            ArrayPool<TParameterless>.Shared.Return(array3);
            ArrayPool<TParameters>.Shared.Return(array4);
            ArrayPool<TParameters>.Shared.Return(array5);
            ArrayPool<TParameters>.Shared.Return(array6);
            ArrayPool<TParameters>.Shared.Return(array7);
        }
    }
}