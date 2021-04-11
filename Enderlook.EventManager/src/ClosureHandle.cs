using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class HeapClosureHandleBase<TEvent> : IDisposable
    {
        public abstract void Raise(TEvent argument);

        public abstract void Purge();

        public abstract void Dispose();
    }

    internal sealed class HeapClosureHandle<TClosure, TEvent> : HeapClosureHandleBase<TEvent>
    {
        public ClosureHandle<TClosure, TEvent> handle = ClosureHandle<TClosure, TEvent>.Create();

        public override void Raise(TEvent argument) => handle.Raise(argument);

        public override void Purge() => handle.Purge();

        public override void Dispose() => handle.Dispose();
    }

    internal struct ClosureHandle<TClosure, TEvent> : IDisposable
    {
        public EventList<ClosureDelegate<TClosure, Parameterless>> parameterless;
        public EventList<ClosureDelegate<TClosure, TEvent>> parameters;
        public EventListOnce<ClosureDelegate<TClosure, Parameterless>> parameterlessOnce;
        public EventListOnce<ClosureDelegate<TClosure, TEvent>> parametersOnce;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ClosureHandle<TClosure, TEvent> Create() => new ClosureHandle<TClosure, TEvent>()
        {
            parameterless = EventList<ClosureDelegate<TClosure, Parameterless>>.Create(),
            parameters = EventList<ClosureDelegate<TClosure, TEvent>>.Create(),
            parameterlessOnce = EventListOnce<ClosureDelegate<TClosure, Parameterless>>.Create(),
            parametersOnce = EventListOnce<ClosureDelegate<TClosure, TEvent>>.Create(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TClosure> @delegate, TClosure closure)
            => parameterless.Add(new ClosureDelegate<TClosure, Parameterless>(Unsafe.As<Action<TClosure, Parameterless>>(@delegate), closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TClosure> @delegate, TClosure closure)
            => parameterless.Remove(new ClosureDelegate<TClosure, Parameterless>(Unsafe.As<Action<TClosure, Parameterless>>(@delegate), closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parameters.Add(new ClosureDelegate<TClosure, TEvent>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parameters.Remove(new ClosureDelegate<TClosure, TEvent>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TClosure> @delegate, TClosure closure)
            => parameterlessOnce.Add(new ClosureDelegate<TClosure, Parameterless>(Unsafe.As<Action<TClosure, Parameterless>>(@delegate), closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TClosure> @delegate, TClosure closure)
            => parameterlessOnce.Remove(new ClosureDelegate<TClosure, Parameterless>(Unsafe.As<Action<TClosure, Parameterless>>(@delegate), closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parametersOnce.Add(new ClosureDelegate<TClosure, TEvent>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parametersOnce.Remove(new ClosureDelegate<TClosure, TEvent>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Purge()
            => Utility.Purge(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
            => Utility.Raise(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce, argument);
    }
}