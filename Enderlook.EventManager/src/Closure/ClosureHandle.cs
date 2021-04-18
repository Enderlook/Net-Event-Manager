
using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct ClosureHandle<TClosure, TEvent> : IDisposable
    {
        /* We store instances of `Action<TClosure>`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TClosure>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<ClosureDelegate<TClosure>> parameterless;
        private EventListOnce<ClosureDelegate<TClosure>> parameterlessOnce;

        /* We store instances of `Action<TClosure, TEvent>`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TClosure, TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<ClosureDelegate<TClosure>> parameters;
        private EventListOnce<ClosureDelegate<TClosure>> parametersOnce;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ClosureHandle<TClosure, TEvent> Create() => new()
        {
            parameterless = EventList<ClosureDelegate<TClosure>>.Create(),
            parameterlessOnce = EventListOnce<ClosureDelegate<TClosure>>.Create(),
            parameters = EventList<ClosureDelegate<TClosure>>.Create(),
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

        public void Dispose()
        {
            parameterless.Dispose();
            parameterlessOnce.Dispose();
            parameters.Dispose();
            parametersOnce.Dispose();
        }

        public void Purge()
        {
            parameterless.Purge();
            parameterlessOnce.Purge();
            parameters.Purge();
            parametersOnce.Purge();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(in HandleSnapshoot handleSnapshoot, TEvent argument)
            => handleSnapshoot.Raise<ClosureDelegate<TClosure>, ClosureDelegate<TClosure>, TEvent, HasClosure, TClosure>(argument);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(in HandleSnapshoot handleSnapshoot)
            => handleSnapshoot.Return(ref parameterless, ref parameters);

        public HandleSnapshoot ExtractSnapshoot()
            => HandleSnapshoot.Create(
                parameterless.GetExecutionList(),
                parameterlessOnce.GetExecutionList(),
                parameters.GetExecutionList(),
                parametersOnce.GetExecutionList()
            );
    }
}