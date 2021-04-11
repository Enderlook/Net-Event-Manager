using System;
using System.Buffers;
using System.Collections.Generic;
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
        public static void Add<T>(ref T[] array, ref int count, T element)
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
        public static void Swap<T>(ref T[] array, ref int count, ref T[] newArray, out int newCount)
        {
            T[] array_ = Steal(ref array);

            newCount = count;
            count = 0;
            array = newArray;
            newArray = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Extract<T>(ref T[] array, ref int count, out T[] newArray, out int newCount)
        {
            T[] array_ = Steal(ref array);

            newCount = count;
            count = 0;
            array = ArrayPool<T>.Shared.Rent(newCount);
            newArray = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractToRun<T>(
            ref T[] toRun, ref int toRunCount, ref T[] toRemove, ref int toRemoveCount,
            out T[] toRunExtracted, out int toRunExtractedCount)
        {
            Extract(ref toRun, ref toRunCount, out toRunExtracted, out toRunExtractedCount);
            Extract(ref toRemove, ref toRemoveCount, out T[] toRemoveExtracted, out int countRemove);
            if (countRemove > 0)
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                T _ = toRunExtracted[toRunExtractedCount];
                _ = toRemoveExtracted[countRemove];
                for (int i = 0; i < toRunExtractedCount; i++)
                {
                    T element = toRunExtracted[i];
                    for (int k = countRemove - 1; k >= 0; k--)
                    {
                        if (comparer.Equals(element, toRemoveExtracted[k]))
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Drain<T>(ref T[] destination, ref int destiantionCount, T[] source, int sourceCount)
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
                Array.Clear(from, 0, fromCount);
                ArrayPool<T>.Shared.Return(from);
                destiantionCount = totalCount;
                destination = newArray;
            }

            Array.Clear(source, 0, sourceCount);
            ArrayPool<T>.Shared.Return(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Execute<TDelegate, TEvent, TMode, TClosure>(TDelegate @delegate, TEvent argument)
        {
            if (typeof(TMode) == typeof(IsSimple))
            {
                Debug.Assert(typeof(TClosure) == typeof(Unused));
                if (typeof(TEvent) == typeof(HasNoParameter))
                {
                    Debug.Assert(@delegate is Action);
                    Unsafe.As<Action>(@delegate)();
                }
                else
                {
                    Debug.Assert(@delegate is Action<TEvent>);
                    Unsafe.As<Action<TEvent>>(@delegate)(argument);
                }
            }
            else if (typeof(TMode) == typeof(IsClosure))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));

                Debug.Assert(@delegate is ClosureDelegate<TClosure>);
                ClosureDelegate<TClosure> closure = Unsafe.As<TDelegate, ClosureDelegate<TClosure>>(ref @delegate);

                if (typeof(TEvent) == typeof(HasNoParameter))
                {
                    Debug.Assert(closure.@delegate is Action<TClosure>);
                    Unsafe.As<Action<TClosure>>(closure.@delegate)(closure.closure);
                }
                else
                {
                    Debug.Assert(closure.@delegate is Action<TClosure, TEvent>);
                    Unsafe.As<Action<TClosure, TEvent>>(closure.@delegate)(closure.closure, argument);
                }
            }
            Debug.Fail("Impossible state.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InnerRaise<TDelegate, TEvent, TMode, TClosure>(ref TDelegate[] a, int count, TEvent argument)
        {
            TDelegate _ = a[count];
            for (int i = 0; i < count; i++)
                Execute<TDelegate, TEvent, TMode, TClosure>(a[i], argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InnerRaise<TDelegate, TEvent, TMode, TClosure>(ref TDelegate[] a, ref TDelegate[] b, int count, int countRemove, TEvent argument)
        {
            TDelegate _ = a[count];
            _ = b[countRemove];
            EqualityComparer<TDelegate> comparer = EqualityComparer<TDelegate>.Default;
            for (int i = 0; i < count; i++)
            {
                TDelegate element = a[i];
                for (int j = countRemove - 1; j >= 0; j--)
                {
                    if (comparer.Equals(element, b[j]))
                    {
                        Array.Copy(b, j + 1, b, j, countRemove - j);
                        countRemove--;
                        goto next;
                    }
                }

                Execute<TDelegate, TEvent, TMode, TClosure>(element, argument);

                next:;
            }
        }

        public static void Raise<TEvent, TDelegate, TMode, TClosure>(
            ref EventList<TDelegate> parameterless,
            ref EventList<TDelegate> parameters,
            ref EventListOnce<TDelegate> parameterlessOnce,
            ref EventListOnce<TDelegate> parametersOnce,
            TEvent argument)
        {
            parameterless.ExtractToRun(out TDelegate[] parameterless1, out int parameterlessCount1);
            parameterlessOnce.ExtractToRun(out TDelegate[] parameterlessOnce1, out int parameterlessOnceCount1, out TDelegate[] parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(out TDelegate[] parameters1, out int parametersCount1);
            parametersOnce.ExtractToRun(out TDelegate[] parametersOnce1, out int parametersOnceCount1, out TDelegate[] parametersOnce2, out int parametersOnceCount2);

            InnerRaise<TDelegate, HasNoParameter, TMode, TClosure>(ref parameterless1, parameterlessCount1, new HasNoParameter());
            InnerRaise<TDelegate, HasNoParameter, TMode, TClosure>(ref parameterlessOnce1, parametersCount1, new HasNoParameter());
            InnerRaise<TDelegate, TEvent, TMode, TClosure>(ref parameters1, parametersCount1, argument);
            InnerRaise<TDelegate, TEvent, TMode, TClosure>(ref parametersOnce1, parametersOnceCount1, argument);

            parameterless.InjectToRun(parameterless1, parameterlessCount1);
            parameterlessOnce.InjectToRun(parameterlessOnce1, parameterlessOnceCount1);
            parameters.InjectToRun(parameters1, parametersCount1);
            parametersOnce.InjectToRun(parametersOnce1, parametersOnceCount1);
        }

        public static void Purge<TDelegate>(
            ref EventList<TDelegate> parameterless,
            ref EventList<TDelegate> parameters,
            ref EventListOnce<TDelegate> parameterlessOnce,
            ref EventListOnce<TDelegate> parametersOnce)
        {
            parameterless.ExtractToRun(out TDelegate[] parameterless1, out int parameterlessCount1);
            parameterless.InjectToRun(parameterless1, parameterlessCount1);
            parameterlessOnce.ExtractToRunRemoved(out TDelegate[] parameterlessOnce1, out int parameterlessOnceCount1);
            parameterlessOnce.InjectToRun(parameterlessOnce1, parameterlessOnceCount1);

            parameters.ExtractToRun(out TDelegate[] parameters1, out int parametersCount1);
            parameters.InjectToRun(parameters1, parametersCount1);
            parametersOnce.ExtractToRunRemoved(out TDelegate[] parametersOnce1, out int parametersOnceCount1);
            parametersOnce.InjectToRun(parametersOnce1, parametersOnceCount1);
        }
    }
}