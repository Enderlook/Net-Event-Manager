
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct ClosureHandle<TClosure, TEvent> : IDisposable
    {
        /* We store instances of `Action`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventList<ClosureDelegate<TClosure>> parameterless;
        private EventList<ClosureDelegate<TClosure>> parameters;

        /* We store instances of `Action<TEvent>`.
         * The lack of the delegate type in the `ClosureDelegate` type allow us to avoid the generic instantiation of ArrayPool<Action<TEvent>>.
         * And so we store less unused arrays on the pool.*/
        private EventListOnce<ClosureDelegate<TClosure>> parameterlessOnce;
        private EventListOnce<ClosureDelegate<TClosure>> parametersOnce;

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
            parameterless.ExtractToRun(out ClosureDelegate<TClosure>[] parameterless1, out int parameterlessCount1);
            parameterlessOnce.ExtractToRun(out ClosureDelegate<TClosure>[] parameterlessOnce1, out int parameterlessOnceCount1, out ClosureDelegate<TClosure>[] parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(out ClosureDelegate<TClosure>[] parameters1, out int parametersCount1);
            parametersOnce.ExtractToRun(out ClosureDelegate<TClosure>[] parametersOnce1, out int parametersOnceCount1, out ClosureDelegate<TClosure>[] parametersOnce2, out int parametersOnceCount2);

            Utility.Raise<TEvent, ClosureDelegate<TClosure>, IsClosure, TClosure>(
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
            Debug.Assert(handleSnapshoot.parameterless1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameterlessOnce1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameterlessOnce2 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameters1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parametersOnce1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parametersOnce2 is ClosureDelegate<TClosure>[]);

            ClosureDelegate<TClosure>[] parameterless1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterless1);
            ClosureDelegate<TClosure>[] parameterlessOnce1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterlessOnce1);
            ClosureDelegate<TClosure>[] parameterlessOnce2 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterlessOnce2);
            ClosureDelegate<TClosure>[] parameters1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameters1);
            ClosureDelegate<TClosure>[] parametersOnce1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parametersOnce1);
            ClosureDelegate<TClosure>[] parametersOnce2 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parametersOnce2);

            Utility.Raise<TEvent, ClosureDelegate<TClosure>, IsClosure, TClosure>(
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
            Debug.Assert(handleSnapshoot.parameterless1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameterlessOnce1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameterlessOnce2 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parameters1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parametersOnce1 is ClosureDelegate<TClosure>[]);
            Debug.Assert(handleSnapshoot.parametersOnce2 is ClosureDelegate<TClosure>[]);

            ClosureDelegate<TClosure>[] parameterless1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterless1);
            ClosureDelegate<TClosure>[] parameterlessOnce1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterlessOnce1);
            ClosureDelegate<TClosure>[] parameterlessOnce2 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameterlessOnce2);
            ClosureDelegate<TClosure>[] parameters1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parameters1);
            ClosureDelegate<TClosure>[] parametersOnce1 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parametersOnce1);
            ClosureDelegate<TClosure>[] parametersOnce2 = Unsafe.As<ClosureDelegate<TClosure>[]>(handleSnapshoot.parametersOnce2);

            Utility.CleanAfterRaise(
                ref parameterless, ref parameters,
                parameterless1, handleSnapshoot.parameterlessCount1, parameterlessOnce1, handleSnapshoot.parameterlessOnceCount1,
                parameterlessOnce2, handleSnapshoot.parameterlessOnceCount2,
                parameters1, handleSnapshoot.parametersCount1, parametersOnce1, handleSnapshoot.parametersOnceCount1,
                parametersOnce2, handleSnapshoot.parametersOnceCount2);
        }
    }
}