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
        /* We store instances of `Action`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        public EventList<ClosureDelegate<TClosure>> parameterless;
        public EventList<ClosureDelegate<TClosure>> parameters;

        /* We store instances of `Action<TEvent>`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        public EventListOnce<ClosureDelegate<TClosure>> parameterlessOnce;
        public EventListOnce<ClosureDelegate<TClosure>> parametersOnce;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ClosureHandle<TClosure, TEvent> Create() => new ClosureHandle<TClosure, TEvent>()
        {
            parameterless = EventList<ClosureDelegate<TClosure>>.Create(),
            parameters = EventList<ClosureDelegate<TClosure>>.Create(),
            parameterlessOnce = EventListOnce<ClosureDelegate<TClosure>>.Create(),
            parametersOnce = EventListOnce<ClosureDelegate<TClosure>>.Create(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TClosure> @delegate, TClosure closure)
            => parameterless.Add(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TClosure> @delegate, TClosure closure)
            => parameterless.Remove(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parameters.Add(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parameters.Remove(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TClosure> @delegate, TClosure closure)
            => parameterlessOnce.Add(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TClosure> @delegate, TClosure closure)
            => parameterlessOnce.Remove(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubscribeOnce(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parametersOnce.Add(new ClosureDelegate<TClosure>(@delegate, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsubscribeOnce(Action<TClosure, TEvent> @delegate, TClosure closure)
            => parametersOnce.Remove(new ClosureDelegate<TClosure>(@delegate, closure));

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
            => Utility.Raise<TEvent, ClosureDelegate<TClosure>, IsClosure, TClosure>(
                ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce, argument);
    }
}