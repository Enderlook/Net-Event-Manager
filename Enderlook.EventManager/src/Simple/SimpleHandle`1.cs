using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class SimpleHandle<TEvent> : SimpleHandle
    {
        private EventList<Action> parameterless = EventList<Action>.Create();
        private EventListOnce<Action> parameterlessOnce = EventListOnce<Action>.Create();

        private EventList<Action<TEvent>> parameters = EventList<Action<TEvent>>.Create();
        private EventListOnce<Action<TEvent>> parametersOnce = EventListOnce<Action<TEvent>>.Create();

        private ClosureHandle<object, TEvent> referenceClosures = ClosureHandle<object, TEvent>.Create();

        private Array<HeapClosureHandleBase<TEvent>> valueClosures = Array<HeapClosureHandleBase<TEvent>>.Empty();
        private int valueClosuresCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action @delegate)
            => parameterless.Add(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action @delegate)
            => parameterless.Remove(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TEvent> @delegate)
            => parameters.Add(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TEvent> @delegate)
            => parameters.Remove(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action @delegate)
            => parameterlessOnce.Add(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action @delegate)
            => parameterlessOnce.Remove(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TEvent> @delegate)
            => parametersOnce.Add(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TEvent> @delegate)
            => parametersOnce.Remove(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Subscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Unsubscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe<TClosure, T>(Action<TClosure, T> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Subscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Unsubscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Subscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Unsubscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Subscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            Debug.Assert(!typeof(TClosure).IsValueType);
            referenceClosures.Unsubscribe(Unsafe.As<Action<object>>(@delegate), closure);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValueClosure(HeapClosureHandleBase<TEvent> closure)
            => Utility.Add(ref valueClosures, ref valueClosuresCount, closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            lock (this)
            {
                int handles = 0;
                handles++; // SimpleHandle
                handles++; // Reference Closures Handles
                handles += valueClosuresCount;

                Array<HandleSnapshoot> snapshoots = Array<HandleSnapshoot>.Rent(handles);
                try
                {
                    // Create snapshoot of listeners.
                    int index = 0;
                    snapshoots[index++] = HandleSnapshoot.Create(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);
                    snapshoots[index++] = referenceClosures.ExtractSnapshoot();
                    int i = 0;
                    for (; i < valueClosuresCount; i++)
                        snapshoots[index++] = valueClosures[i].ExtractSnapshoot();

                    try
                    {
                        index = i = 0;
                        snapshoots[index++].Raise<TEvent, Action, Action<TEvent>, IsSimple, Unused>(ref parameterless, ref parameters, argument);
                        referenceClosures.Raise(snapshoots[index++], argument);
                        for (; i < valueClosuresCount; i++)
                            valueClosures[i].Raise(snapshoots[index++], argument);
                    }
                    catch
                    {
                        ClearOnError(snapshoots, ref index, ref i);
                    }
                }
                finally
                {
                    snapshoots.ClearAndReturn(handles);
                }
            }

            void ClearOnError(Array<HandleSnapshoot> snapshoots, ref int index, ref int i)
            {
                // Even if an event crash, we can't just loose all registered listeners.
                // That is why this is inside a try/catch.
                // However, we don't check the snapshoot of this instance because that is already treated inside Raise.
                // This is in a catch instead of finally, because the Raise method already produces the cleaning

                if (index == 1)
                    referenceClosures.ClearAfterRaise(snapshoots[index++]);

                for (; i < valueClosuresCount; i++)
                    valueClosures[i].ClearAfterRaise(snapshoots[index++]);
            }
        }

        public override void Dispose()
        {
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();

            referenceClosures.Dispose();

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Dispose();

            valueClosures.ClearAndReturn(valueClosuresCount);
        }

        public override void Purge()
        {
            Utility.Purge(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);

            referenceClosures.Purge();

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Purge();
        }
    }
}