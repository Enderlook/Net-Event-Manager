using System;
using System.Runtime.CompilerServices;
using System.Threading;

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
        private static readonly ClosureDelegate<TClosure, Parameterless>[] globalParameterlessEmpty = new ClosureDelegate<TClosure, Parameterless>[0];
        private static ClosureDelegate<TClosure, Parameterless>[] globalParameterless1 = globalParameterlessEmpty;
        private static ClosureDelegate<TClosure, Parameterless>[] globalParameterless2 = globalParameterlessEmpty;
        private static ClosureDelegate<TClosure, Parameterless>[] globalParameterless3 = globalParameterlessEmpty;
        private static ClosureDelegate<TClosure, Parameterless>[] globalParameterless4 = globalParameterlessEmpty;

        private static readonly ClosureDelegate<TClosure, TEvent>[] globalParametersEmpty = new ClosureDelegate<TClosure, TEvent>[0];
        private static ClosureDelegate<TClosure, TEvent>[] globalParameters1 = globalParametersEmpty;
        private static ClosureDelegate<TClosure, TEvent>[] globalParameters2 = globalParametersEmpty;
        private static ClosureDelegate<TClosure, TEvent>[] globalParameters3 = globalParametersEmpty;
        private static ClosureDelegate<TClosure, TEvent>[] globalParameters4 = globalParametersEmpty;

        public EventList<ClosureDelegate<TClosure, Parameterless>, Parameterless> parameterless;
        public EventList<ClosureDelegate<TClosure, TEvent>, TEvent> parameters;
        public EventListOnce<ClosureDelegate<TClosure, Parameterless>, Parameterless> parameterlessOnce;
        public EventListOnce<ClosureDelegate<TClosure, TEvent>, TEvent> parametersOnce;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ClosureHandle<TClosure, TEvent> Create() => new ClosureHandle<TClosure, TEvent>()
        {
            parameterless = EventList<ClosureDelegate<TClosure, Parameterless>, Parameterless>.Create(),
            parameters = EventList<ClosureDelegate<TClosure, TEvent>, TEvent>.Create(),
            parameterlessOnce = EventListOnce<ClosureDelegate<TClosure, Parameterless>, Parameterless>.Create(),
            parametersOnce = EventListOnce<ClosureDelegate<TClosure, TEvent>, TEvent>.Create(),
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
        {
            ClosureDelegate<TClosure, Parameterless>[] a = Interlocked.Exchange(ref globalParameterless1, globalParameterlessEmpty);
            ClosureDelegate<TClosure, Parameterless>[] b = Interlocked.Exchange(ref globalParameterless2, globalParameterlessEmpty);
            ClosureDelegate<TClosure, TEvent>[] c = Interlocked.Exchange(ref globalParameters1, globalParametersEmpty);
            ClosureDelegate<TClosure, TEvent>[] d = Interlocked.Exchange(ref globalParameters2, globalParametersEmpty);

            parameterless.ExtractToRun(ref a, ref b, out int count);
            parameterless.InjectToRun(ref a, count);
            parameters.ExtractToRun(ref c, ref d, out count);
            parameters.InjectToRun(ref c, count);
            parameterlessOnce.ExtractToRunRemoved(ref a, ref b, out count);
            parameterlessOnce.InjectToRun(ref a, count);
            parametersOnce.ExtractToRunRemoved(ref c, ref d, out count);
            parametersOnce.InjectToRun(ref c, count);

            Interlocked.Exchange(ref globalParameterless1, a);
            Interlocked.Exchange(ref globalParameterless2, b);
            Interlocked.Exchange(ref globalParameters1, c);
            Interlocked.Exchange(ref globalParameters2, d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            ClosureDelegate<TClosure, Parameterless>[] pl1 = Interlocked.Exchange(ref globalParameterless1, globalParameterlessEmpty);
            ClosureDelegate<TClosure, Parameterless>[] pl2 = Interlocked.Exchange(ref globalParameterless2, globalParameterlessEmpty);
            ClosureDelegate<TClosure, Parameterless>[] plo1 = Interlocked.Exchange(ref globalParameterless3, globalParameterlessEmpty);
            ClosureDelegate<TClosure, Parameterless>[] plo2 = Interlocked.Exchange(ref globalParameterless4, globalParameterlessEmpty);
            ClosureDelegate<TClosure, TEvent>[] pf1 = Interlocked.Exchange(ref globalParameters1, globalParametersEmpty);
            ClosureDelegate<TClosure, TEvent>[] pf2 = Interlocked.Exchange(ref globalParameters2, globalParametersEmpty);
            ClosureDelegate<TClosure, TEvent>[] pfo1 = Interlocked.Exchange(ref globalParameters3, globalParametersEmpty);
            ClosureDelegate<TClosure, TEvent>[] pfo2 = Interlocked.Exchange(ref globalParameters4, globalParametersEmpty);

            parameterless.ExtractToRun(ref pl1, ref pl2, out int cl);
            parameterlessOnce.ExtractToRun(ref plo1, ref plo2, out int clo, out int clo2);
            parameters.ExtractToRun(ref pf1, ref pf2, out int cf);
            parametersOnce.ExtractToRun(ref pf1, ref pf2, out int cfo, out int cfo2);

            Utility.InnerRaise(ref parameterless, ref pl1, cl, new Parameterless());
            Utility.InnerRaise(ref plo1, ref plo2, clo, clo2, new Parameterless());
            Utility.InnerRaise(ref parameters, ref pf1, cf, argument);
            Utility.InnerRaise(ref pfo1, ref pfo2, cfo, cfo2, argument);

            Interlocked.Exchange(ref globalParameterless1, pl1);
            Interlocked.Exchange(ref globalParameterless2, pl2);
            Interlocked.Exchange(ref globalParameterless2, plo1);
            Interlocked.Exchange(ref globalParameterless2, plo2);
            Interlocked.Exchange(ref globalParameters1, pf1);
            Interlocked.Exchange(ref globalParameters1, pf2);
            Interlocked.Exchange(ref globalParameters1, pfo1);
            Interlocked.Exchange(ref globalParameters1, pfo2);
        }
    }
}