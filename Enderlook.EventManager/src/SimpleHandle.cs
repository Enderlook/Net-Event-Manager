using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class SimpleHandle : IDisposable
    {
        public abstract void Dispose();

        public abstract void Purge();
    }

    internal sealed class SimpleHandle<TEvent> : SimpleHandle
    {
        private static readonly HeapClosureHandleBase<TEvent>[] empty = new HeapClosureHandleBase<TEvent>[0];

        private EventList<SimpleDelegate<Parameterless>, Parameterless> parameterless = EventList<SimpleDelegate<Parameterless>, Parameterless>.Create();
        private EventList<SimpleDelegate<TEvent>, TEvent> parameters = EventList<SimpleDelegate<TEvent>, TEvent>.Create();
        private EventListOnce<SimpleDelegate<Parameterless>, Parameterless> parameterlessOnce = EventListOnce<SimpleDelegate<Parameterless>, Parameterless>.Create();
        private EventListOnce<SimpleDelegate<TEvent>, TEvent> parametersOnce = EventListOnce<SimpleDelegate<TEvent>, TEvent>.Create();
        private ClosureHandle<object, TEvent> referenceClosures = ClosureHandle<object, TEvent>.Create();
        private HeapClosureHandleBase<TEvent>[] valueClosures = empty;
        private int valueClosuresCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action @delegate)
            => parameterless.Add(new SimpleDelegate<Parameterless>(Unsafe.As<Action<Parameterless>>(@delegate)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action @delegate)
            => parameterless.Remove(new SimpleDelegate<Parameterless>(Unsafe.As<Action<Parameterless>>(@delegate)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TEvent> @delegate)
            => parameters.Add(new SimpleDelegate<TEvent>(@delegate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TEvent> @delegate)
            => parameters.Remove(new SimpleDelegate<TEvent>(@delegate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action @delegate)
            => parameterlessOnce.Add(new SimpleDelegate<Parameterless>(Unsafe.As<Action<Parameterless>>(@delegate)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action @delegate)
            => parameterlessOnce.Remove(new SimpleDelegate<Parameterless>(Unsafe.As<Action<Parameterless>>(@delegate)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TEvent> @delegate)
            => parametersOnce.Add(new SimpleDelegate<TEvent>(@delegate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TEvent> @delegate)
            => parametersOnce.Remove(new SimpleDelegate<TEvent>(@delegate));

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
        public void AddValueClosure(HeapClosureHandleBase<TEvent> closure) => Utility.InnerAdd(ref valueClosures, ref valueClosuresCount, closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            Utility.Raise(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce, argument);

            referenceClosures.Raise(argument);

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Raise(argument);
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