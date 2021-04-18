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

        private List<HeapClosureHandleBase<TEvent>> valueClosures = List<HeapClosureHandleBase<TEvent>>.Empty();

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
            => valueClosures.ConcurrentAdd(closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            int valueClosuresCount = valueClosures.Count;

            int handles = 0;
            handles++; // SimpleHandle
            handles++; // Reference Closures Handles
            handles += valueClosuresCount;

            Array<HandleSnapshoot> snapshoots = Array<HandleSnapshoot>.Rent(handles);

            // Create snapshoot of listeners.
            int index = 0;
            snapshoots[index++] = HandleSnapshoot.Create(
                parameterless.GetExecutionList(),
                parameterlessOnce.GetExecutionList(),
                parameters.GetExecutionList(),
                parametersOnce.GetExecutionList()
            );
            snapshoots[index++] = referenceClosures.ExtractSnapshoot();

            if (valueClosuresCount > 0)
            {
                Array<HeapClosureHandleBase<TEvent>> valueClosuresStolen = Array<HeapClosureHandleBase<TEvent>>.Steal(ref valueClosures.Array);
                int i = 0;
                for (; i < valueClosuresCount; i++)
                    snapshoots[index++] = valueClosures[i].ExtractSnapshoot();
                Array<HeapClosureHandleBase<TEvent>>.Overwrite(ref valueClosures.Array, valueClosuresStolen);
            }

            try
            {
                index = 0;
                snapshoots[index++].Raise<Action, Action<TEvent>, TEvent, HasNoClosure, Unused>(argument);
                referenceClosures.Raise(snapshoots[index++], argument);
                if (valueClosuresCount > 0)
                {
                    for (int i = 0; i < valueClosuresCount; i++)
                        valueClosures.ConcurrentGet(i).Raise(snapshoots[index++], argument);
                }
            }
            finally
            {
                index = 0;
                snapshoots[index++].Return(ref parameterless, ref parameters);
                referenceClosures.Return(snapshoots[index++]);

                Array<HeapClosureHandleBase<TEvent>> valueClosuresStolen = Array<HeapClosureHandleBase<TEvent>>.Steal(ref valueClosures.Array);
                for (int i = 0; i < valueClosuresCount; i++)
                    valueClosuresStolen[i].Return(snapshoots[index++]);
                Array<HeapClosureHandleBase<TEvent>>.Overwrite(ref valueClosures.Array, valueClosuresStolen);

                snapshoots.ClearIfContainsReferences(handles);
                snapshoots.Return();
            }
        }

        public override void Dispose()
        {
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();
            referenceClosures.Dispose();

            Array<HeapClosureHandleBase<TEvent>> valueClosuresStolen = Array<HeapClosureHandleBase<TEvent>>.Steal(ref valueClosures.Array);
            int valueClosuresCount = valueClosures.Count;

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Dispose();

            Array<HeapClosureHandleBase<TEvent>>.Overwrite(ref valueClosures.Array, valueClosuresStolen);
            valueClosures.Return();
        }

        public override void Purge()
        {
            parameters.Purge();
            parameterless.Purge();
            parametersOnce.Purge();
            parameterlessOnce.Purge();
            referenceClosures.Purge();

            Array<HeapClosureHandleBase<TEvent>> valueClosuresStolen = Array<HeapClosureHandleBase<TEvent>>.Steal(ref valueClosures.Array);
            int valueClosuresCount = valueClosures.Count;

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Purge();

            Array<HeapClosureHandleBase<TEvent>>.Overwrite(ref valueClosures.Array, valueClosuresStolen);
        }
    }
}