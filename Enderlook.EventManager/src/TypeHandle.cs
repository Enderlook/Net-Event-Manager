using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal abstract class TypeHandle : IDisposable
    {
        public abstract void Dispose();

        public abstract void Purge();
    }

    internal sealed class TypeHandle<TEvent> : TypeHandle
    {
        private static readonly HeapClosureHandleBase<TEvent>[] empty = new HeapClosureHandleBase<TEvent>[0];

        private static readonly SimpleDelegate<Parameterless>[] globalParameterlessEmpty = new SimpleDelegate<Parameterless>[0];
        private static SimpleDelegate<Parameterless>[] globalParameterless1 = globalParameterlessEmpty;
        private static SimpleDelegate<Parameterless>[] globalParameterless2 = globalParameterlessEmpty;
        private static SimpleDelegate<Parameterless>[] globalParameterless3 = globalParameterlessEmpty;
        private static SimpleDelegate<Parameterless>[] globalParameterless4 = globalParameterlessEmpty;

        private static readonly SimpleDelegate<TEvent>[] globalParametersEmpty = new SimpleDelegate<TEvent>[0];
        private static SimpleDelegate<TEvent>[] globalParameters1 = globalParametersEmpty;
        private static SimpleDelegate<TEvent>[] globalParameters2 = globalParametersEmpty;
        private static SimpleDelegate<TEvent>[] globalParameters3 = globalParametersEmpty;
        private static SimpleDelegate<TEvent>[] globalParameters4 = globalParametersEmpty;

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

        public void Raise(TEvent argument)
        {
            SimpleDelegate<Parameterless>[] pl1 = Interlocked.Exchange(ref globalParameterless1, globalParameterlessEmpty);
            SimpleDelegate<Parameterless>[] pl2 = Interlocked.Exchange(ref globalParameterless2, globalParameterlessEmpty);
            SimpleDelegate<Parameterless>[] plo1 = Interlocked.Exchange(ref globalParameterless3, globalParameterlessEmpty);
            SimpleDelegate<Parameterless>[] plo2 = Interlocked.Exchange(ref globalParameterless4, globalParameterlessEmpty);
            SimpleDelegate<TEvent>[] pf1 = Interlocked.Exchange(ref globalParameters1, globalParametersEmpty);
            SimpleDelegate<TEvent>[] pf2 = Interlocked.Exchange(ref globalParameters2, globalParametersEmpty);
            SimpleDelegate<TEvent>[] pfo1 = Interlocked.Exchange(ref globalParameters3, globalParametersEmpty);
            SimpleDelegate<TEvent>[] pfo2 = Interlocked.Exchange(ref globalParameters4, globalParametersEmpty);

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
            SimpleDelegate<Parameterless>[] a = Interlocked.Exchange(ref globalParameterless1, globalParameterlessEmpty);
            SimpleDelegate<Parameterless>[] b = Interlocked.Exchange(ref globalParameterless2, globalParameterlessEmpty);
            SimpleDelegate<TEvent>[] c = Interlocked.Exchange(ref globalParameters1, globalParametersEmpty);
            SimpleDelegate<TEvent>[] d = Interlocked.Exchange(ref globalParameters2, globalParametersEmpty);

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

            referenceClosures.Purge();

            for (int i = 0; i < valueClosuresCount; i++)
                valueClosures[i].Purge();
        }
    }
}