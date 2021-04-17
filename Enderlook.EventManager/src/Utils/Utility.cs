using System;
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
        private static Array<T> Steal<T>(ref Array<T> value)
        {
            object value_;
            do
            {
                value_ = Interlocked.Exchange(ref value.array, null);
            } while (value_ is null);
            return new Array<T>(value_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(ref Array<T> array, ref int count, T element)
        {
            Array<T> array_ = Steal(ref array);

            int count_ = count;
            if (count_ == array_.Length)
            {
                if (count_ == 0)
                    array_ = Array<T>.Rent(INITIAL_CAPACITY);
                else
                {
                    Array<T> newArray = Array<T>.Rent(count_ * GROW_FACTOR);
                    array_.CopyToAndReturn(newArray, count_);
                    array_ = newArray;
                }
            }
            array_[count_++] = element;
            count = count_;
            array = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InjectEmpty<T>(ref Array<T> array, ref int count)
        {
            Array<T> array_ = Steal(ref array);

            int oldCount = count;
            count = 0;
            array = Array<T>.Empty();

            array_.ClearAndReturn(oldCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Extract<T>(ref Array<T> array, ref int count, out Array<T> newArray, out int newCount)
        {
            Array<T> array_ = Steal(ref array);

            newCount = count;
            count = 0;
            array = Array<T>.Rent(newCount);
            newArray = array_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractToRun<T>(
            ref Array<T> toRun, ref int toRunCount, ref Array<T> toRemove, ref int toRemoveCount,
            out Array<T> toRunExtracted, out int toRunExtractedCount)
        {
            Extract(ref toRun, ref toRunCount, out toRunExtracted, out toRunExtractedCount);
            Extract(ref toRemove, ref toRemoveCount, out Array<T> toRemoveExtracted, out int countRemove);
            if (countRemove > 0)
            {
                if (toRunExtractedCount == 0)
                    return;
                Remove(toRunExtracted, ref toRunExtractedCount, toRemoveExtracted, ref countRemove);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Remove(Array<T> toRunExtracted, ref int toRunExtractedCount, Array<T> toRemoveExtracted, ref int countRemove)
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                T _ = toRunExtracted[toRunExtractedCount - 1];
                _ = toRemoveExtracted[countRemove - 1];
                for (int i = 0; i < toRunExtractedCount; i++)
                {
                    T element = toRunExtracted[i];
                    for (int k = countRemove - 1; k >= 0; k--)
                    {
                        if (comparer.Equals(element, toRemoveExtracted[k]))
                        {
                            toRemoveExtracted.CopyTo(k + 1, toRemoveExtracted, k, countRemove - k);
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
        public static void Drain<T>(ref Array<T> destination, ref int destinationCount, Array<T> source, int sourceCount)
        {
            if (sourceCount == 0)
                return;

            Array<T> array_ = Steal(ref destination);

            if (destinationCount == 0)
            {
                destinationCount = sourceCount;
                destination = source;
                array_.Return();
                return;
            }

            Merge(out destination, ref destinationCount, ref source, ref sourceCount, array_);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Merge(out Array<T> destination, ref int destinationCount, ref Array<T> source, ref int sourceCount, Array<T> array_)
            {
                Array<T> from;
                Array<T> to;
                int fromCount;
                int toCount;
                if (source.Length > array_.Length)
                {
                    to = source;
                    toCount = sourceCount;
                    from = array_;
                    fromCount = destinationCount;
                }
                else
                {
                    from = source;
                    fromCount = sourceCount;
                    to = array_;
                    toCount = destinationCount;
                }

                int totalCount = fromCount + toCount;
                if (totalCount <= to.Length)
                {
                    if (fromCount > 0)
                        from.CopyTo(to, toCount + 1, fromCount);
                    source = from;
                    sourceCount = fromCount;
                    destinationCount = totalCount;
                    destination = to;
                }
                else
                    ResizeAndCopy(out destination, out destinationCount, out source, out sourceCount, from, to, fromCount, toCount, totalCount);

                source.ClearAndReturn(sourceCount);

                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ResizeAndCopy(out Array<T> destination, out int destinationCount, out Array<T> source, out int sourceCount, Array<T> from, Array<T> to, int fromCount, int toCount, int totalCount)
                {
                    Debug.Assert(fromCount > 0);
                    Array<T> newArray = Array<T>.Rent(totalCount * GROW_FACTOR);
                    from.CopyTo(newArray, fromCount);
                    to.CopyTo(newArray, fromCount + 1, toCount);
                    source = to;
                    sourceCount = toCount;
                    from.ClearAndReturn(fromCount);
                    destinationCount = totalCount;
                    destination = newArray;
                }
            }
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
            else
                Debug.Fail("Impossible state.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RaiseArray<TDelegate, TEvent, TMode, TClosure>(ref Array<TDelegate> a, int count, TEvent argument)
        {
            if (count == 0)
                return;

            TDelegate _ = a[count - 1];
            for (int i = 0; i < count; i++)
                Execute<TDelegate, TEvent, TMode, TClosure>(a[i], argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RaiseArray<TDelegate, TEvent, TMode, TClosure>(Array<TDelegate> a, Array<TDelegate> b, int count, int countRemove, TEvent argument)
        {
            if (count == 0)
                return;

            TDelegate _ = a[count - 1];

            EqualityComparer<TDelegate> comparer = EqualityComparer<TDelegate>.Default;

            if (countRemove == 0)
            {
                for (int i = 0; i < count; i++)
                    Execute<TDelegate, TEvent, TMode, TClosure>(a[i], argument);
                return;
            }

            _ = b[countRemove - 1];
            for (int i = 0; i < count; i++)
            {
                TDelegate element = a[i];
                for (int j = countRemove - 1; j >= 0; j--)
                {
                    if (comparer.Equals(element, b[j]))
                    {
                        b.CopyTo(j + 1, b, j, countRemove - j);
                        countRemove--;
                        goto next;
                    }
                }

                Execute<TDelegate, TEvent, TMode, TClosure>(element, argument);

                next:;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<TEvent, TParameterless, TParameters, TMode, TClosure>(
            ref EventList<TParameterless> parameterless, ref EventList<TParameters> parameters,
            TEvent argument,
            ref Array<TParameterless> parameterless1, int parameterlessCount1,
            Array<TParameterless> parameterlessOnce1, int parameterlessOnceCount1, Array<TParameterless> parameterlessOnce2, int parameterlessOnceCount2,
            ref Array<TParameters> parameters1, int parametersCount1,
            Array<TParameters> parametersOnce1, int parametersOnceCount1, Array<TParameters> parametersOnce2, int parametersOnceCount2)
        {
            try
            {
                RaiseArray<TParameterless, HasNoParameter, TMode, TClosure>(ref parameterless1, parameterlessCount1, new HasNoParameter());
                RaiseArray<TParameterless, HasNoParameter, TMode, TClosure>(parameterlessOnce1, parameterlessOnce2, parameterlessOnceCount1, parameterlessOnceCount2, new HasNoParameter());
                RaiseArray<TParameters, TEvent, TMode, TClosure>(ref parameters1, parametersCount1, argument);
                RaiseArray<TParameters, TEvent, TMode, TClosure>(parametersOnce1, parametersOnce2, parametersOnceCount1, parametersOnceCount2, argument);
            }
            finally
            {
                // Even if an event crash, we can't just loose all registered listeners.
                // That is why this is inside a try/finally.
                CleanAfterRaise(ref parameterless, ref parameters,
                                parameterless1, parameterlessCount1,  parameterlessOnce1, parameterlessOnceCount1,
                                parameterlessOnce2, parameterlessOnceCount2,
                                parameters1, parametersCount1, parametersOnce1, parametersOnceCount1,
                                parametersOnce2, parametersOnceCount2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanAfterRaise<TParameterless, TParameters>(
            ref EventList<TParameterless> parameterless, ref EventList<TParameters> parameters,
            Array<TParameterless> parameterless1, int parameterlessCount1,
            Array<TParameterless> parameterlessOnce1, int parameterlessOnceCount1, Array<TParameterless> parameterlessOnce2, int parameterlessOnceCount2,
            Array<TParameters> parameters1, int parametersCount1,
            Array<TParameters> parametersOnce1, int parametersOnceCount1, Array<TParameters> parametersOnce2, int parametersOnceCount2)
        {
            parameterless.InjectToRun(parameterless1, parameterlessCount1);
            parameters.InjectToRun(parameters1, parametersCount1);

            parameterlessOnce1.ClearAndReturn(parameterlessOnceCount1);
            parameterlessOnce2.ClearAndReturn(parameterlessOnceCount2);
            parametersOnce1.ClearAndReturn(parametersOnceCount1);
            parametersOnce2.ClearAndReturn(parametersOnceCount2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Purge<TParameterless, TParameters>(
            ref EventList<TParameterless> parameterless, ref EventList<TParameters> parameters,
            ref EventListOnce<TParameterless> parameterlessOnce, ref EventListOnce<TParameters> parametersOnce)
        {
            parameterless.ExtractToRun(out Array<TParameterless> parameterless1, out int parameterlessCount1);
            parameterless.InjectToRun(parameterless1, parameterlessCount1);
            parameterlessOnce.ExtractToRunRemoved(out Array<TParameterless> parameterlessOnce1, out int parameterlessOnceCount1);
            parameterlessOnce.InjectToRun(parameterlessOnce1, parameterlessOnceCount1);

            parameters.ExtractToRun(out Array<TParameters> parameters1, out int parametersCount1);
            parameters.InjectToRun(parameters1, parametersCount1);
            parametersOnce.ExtractToRunRemoved(out Array<TParameters> parametersOnce1, out int parametersOnceCount1);
            parametersOnce.InjectToRun(parametersOnce1, parametersOnceCount1);
        }
    }
}