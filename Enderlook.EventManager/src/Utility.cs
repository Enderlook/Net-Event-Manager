using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal static class Utility
    {
        private const int INITIAL_CAPACITY = 4;
        private const int GROW_FACTOR = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Get<T>(ref T value) where T : class
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
            T[] array_ = Get(ref array);

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
            T[] array_ = Get(ref array);

            newCount = count;
            count = 0;
            array = newArray;
            newArray = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractToRun<TDelegate, TEvent>(ref TDelegate[] toRun, ref TDelegate[] toRemove, ref int toRunCount, ref int toRemoveCount, ref TDelegate[] toRunExtracted, ref TDelegate[] replacement, out int count)
            where TDelegate : IDelegate<TDelegate, TEvent>
        {
            InnerSwap(ref toRun, ref toRunCount, ref toRunExtracted, out count);
            InnerSwap(ref toRemove, ref toRemoveCount, ref replacement, out int countRemove);
            if (countRemove > 0)
            {
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                TDelegate _ = toRunExtracted[count];
                _ = replacement[countRemove];
                for (int i = 0; i < count; i++)
                {
                    TDelegate element = toRunExtracted[i];
                    for (int k = countRemove - 1; k >= 0; k--)
                    {
                        if (element.Equals(replacement[k]))
                        {
                            Array.Copy(replacement, k + 1, replacement, k, countRemove - k);
                            countRemove--;
                            goto next;
                        }
                        replacement[j++] = element;
                        next:;
                    }
                }
                count = j;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InjectToRun<T>(ref T[] toRun, ref int toRunCount, ref T[] array, int count)
        {
            T[] array_;
            do
            {
                array_ = Interlocked.Exchange(ref toRun, null);
            } while (array_ is null);

            T[] from;
            T[] to;
            int fromCount;
            int toCount;
            if (array.Length > array_.Length)
            {
                to = array;
                toCount = count;
                from = array_;
                fromCount = toRunCount;
            }
            else
            {
                from = array;
                fromCount = count;
                to = array_;
                toCount = toRunCount;
            }

            int totalCount = fromCount + toCount;
            if (totalCount < to.Length)
            {
                Array.Copy(from, 0, to, toCount + 1, fromCount);
                array = from;
                toRunCount = totalCount;
                toRun = to;
            }
            else
            {
                T[] newArray = ArrayPool<T>.Shared.Rent(totalCount * GROW_FACTOR);
                Array.Copy(from, newArray, fromCount);
                Array.Copy(to, 0, newArray, fromCount + 1, toCount);
                array = to;
                ArrayPool<T>.Shared.Return(from);
                toRunCount = totalCount;
                toRun = newArray;
            }
        }

        public static void InnerRaise<TEvent, TDelegate>(ref EventList<TDelegate, TEvent> list, ref TDelegate[] a, int count, TEvent argument)
            where TDelegate : IDelegate<TDelegate, TEvent>
        {
            TDelegate _ = a[count];
            for (int i = 0; i < count; i++)
                a[i].Invoke(argument);
            list.InjectToRun(ref a, count);
        }

        public static void InnerRaise<TEvent, TDelegate>(ref TDelegate[] a, ref TDelegate[] b, int count, int countRemove, TEvent argument)
            where TDelegate : IDelegate<TDelegate, TEvent>
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
                element.Invoke(argument);
                next:;
            }
        }
    }
}