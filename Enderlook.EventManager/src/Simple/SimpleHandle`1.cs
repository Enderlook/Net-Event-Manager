using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class SimpleHandle<TEvent> : SimpleHandle
    {
        private static readonly HeapClosureHandleBase<TEvent>[] empty = new HeapClosureHandleBase<TEvent>[0];

        /* In `Delegate` we actually store instances of `Action`.
         * But this upcast allow us to avoid the generic instantiation of ArrayPool<Action>.
         * And so we store less unused arrays on the pool.*/
        private EventList<Delegate> parameterless = EventList<Delegate>.Create();
        private EventListOnce<Delegate> parameterlessOnce = EventListOnce<Delegate>.Create();

        /* In `Delegate` we actually store instances of `Action<TEvent>`.
         * But this upcast allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<Delegate> parameters = EventList<Delegate>.Create();
        private EventListOnce<Delegate> parametersOnce = EventListOnce<Delegate>.Create();

        private ClosureHandle<object, TEvent> referenceClosures = ClosureHandle<object, TEvent>.Create();

        private HeapClosureHandleBase<TEvent>[] valueClosures = empty;
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
        public void AddValueClosure(HeapClosureHandleBase<TEvent> closure) => Utility.Add(ref valueClosures, ref valueClosuresCount, closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            int handles = 0;
            handles++; // SimpleHandle
            handles++; // Reference Closures Handles
            handles += valueClosuresCount;

            HandleSnapshoot[] snapshoots = ArrayPool<HandleSnapshoot>.Shared.Rent(handles);
            try
            {
                // Create snapshoot of listeners.
                int index = 0;
                snapshoots[index++] = HandleSnapshoot.Create(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);
                snapshoots[index++] = referenceClosures.ExtractSnapshoot();
                for (int i = 0; i < valueClosuresCount; i++)
                    snapshoots[index++] = valueClosures[i].ExtractSnapshoot();

                index = 0;
                snapshoots[index++].Raise<TEvent, Delegate, IsSimple, Unused>(ref parameterless, ref parameters, argument);
                referenceClosures.Raise(snapshoots[index++], argument);
                for (int i = 0; i < valueClosuresCount; i++)
                    valueClosures[i].Raise(snapshoots[index++], argument);
            }
            finally
            {
                ArrayPool<HandleSnapshoot>.Shared.Return(snapshoots);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Dispose()
        {
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();

            referenceClosures.Dispose();

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Purge()
        {
            Utility.Purge(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);

            referenceClosures.Purge();

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Purge();
        }
    }
}