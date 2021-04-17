
using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct ClosureHandle<TClosure, TEvent> : IDisposable
    {
        /* We store instances of `Action`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<ClosureDelegate<TClosure>> parameterless;
        private EventListOnce<ClosureDelegate<TClosure>> parameterlessOnce;

        /* We store instances of `Action<TEvent>`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<ClosureDelegate<TClosure>> parameters;
        private EventListOnce<ClosureDelegate<TClosure>> parametersOnce;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ClosureHandle<TClosure, TEvent> Create() => new ClosureHandle<TClosure, TEvent>()
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
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();
        }

        public void Purge()
            => Utility.Purge(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            parameterless.ExtractToRun(out Array<ClosureDelegate<TClosure>> parameterless1, out int parameterlessCount1);
            parameterlessOnce.ExtractToRun(out Array<ClosureDelegate<TClosure>> parameterlessOnce1, out int parameterlessOnceCount1, out Array<ClosureDelegate<TClosure>> parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(out Array<ClosureDelegate<TClosure>> parameters1, out int parametersCount1);
            parametersOnce.ExtractToRun(out Array<ClosureDelegate<TClosure>> parametersOnce1, out int parametersOnceCount1, out Array<ClosureDelegate<TClosure>> parametersOnce2, out int parametersOnceCount2);

            Utility.Raise<TEvent, ClosureDelegate<TClosure>, ClosureDelegate<TClosure>, IsClosure, TClosure>(
                ref parameterless, ref parameters,
                argument,
                ref parameterless1, parameterlessCount1,
                parameterlessOnce1, parameterlessOnceCount1, parameterlessOnce2, parameterlessOnceCount2,
                ref parameters1, parametersCount1,
                parametersOnce1, parametersOnceCount1, parametersOnce2, parametersOnceCount2
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HandleSnapshoot ExtractSnapshoot()
            => HandleSnapshoot.Create(ref parameterless, ref parameters, ref parameterlessOnce, ref parametersOnce);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(in HandleSnapshoot handleSnapshoot, TEvent argument)
        {
            Array<ClosureDelegate<TClosure>> parameterless1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterless1);
            Array<ClosureDelegate<TClosure>> parameterlessOnce1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterlessOnce1);
            Array<ClosureDelegate<TClosure>> parameterlessOnce2 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterlessOnce2);
            Array<ClosureDelegate<TClosure>> parameters1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameters1);
            Array<ClosureDelegate<TClosure>> parametersOnce1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parametersOnce1);
            Array<ClosureDelegate<TClosure>> parametersOnce2 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parametersOnce2);

            Utility.Raise<TEvent, ClosureDelegate<TClosure>, ClosureDelegate<TClosure>, IsClosure, TClosure>(
                ref parameterless, ref parameters,
                argument,
                ref parameterless1, handleSnapshoot.parameterlessCount1,
                parameterlessOnce1, handleSnapshoot.parameterlessOnceCount1, parameterlessOnce2, handleSnapshoot.parameterlessOnceCount2,
                ref parameters1, handleSnapshoot.parametersCount1,
                parametersOnce1, handleSnapshoot.parametersOnceCount1, parametersOnce2, handleSnapshoot.parametersOnceCount2
            );
        }

        public void ClearAfterRaise(in HandleSnapshoot handleSnapshoot)
        {
            Array<ClosureDelegate<TClosure>> parameterless1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterless1);
            Array<ClosureDelegate<TClosure>> parameterlessOnce1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterlessOnce1);
            Array<ClosureDelegate<TClosure>> parameterlessOnce2 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameterlessOnce2);
            Array<ClosureDelegate<TClosure>> parameters1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parameters1);
            Array<ClosureDelegate<TClosure>> parametersOnce1 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parametersOnce1);
            Array<ClosureDelegate<TClosure>> parametersOnce2 = new Array<ClosureDelegate<TClosure>>(handleSnapshoot.parametersOnce2);

            Utility.CleanAfterRaise(
                ref parameterless, ref parameters,
                parameterless1, handleSnapshoot.parameterlessCount1, parameterlessOnce1, handleSnapshoot.parameterlessOnceCount1,
                parameterlessOnce2, handleSnapshoot.parameterlessOnceCount2,
                parameters1, handleSnapshoot.parametersCount1, parametersOnce1, handleSnapshoot.parametersOnceCount1,
                parametersOnce2, handleSnapshoot.parametersOnceCount2);
        }
    }
}