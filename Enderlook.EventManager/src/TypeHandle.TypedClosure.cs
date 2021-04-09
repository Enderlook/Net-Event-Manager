using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle
    {
        private abstract class TypedClosure
        {
            public abstract void Dispose();
            public abstract void Purge();
            public abstract void Raise<T>(T argument);
        }

        private sealed class TypedClosure<TClosure> : TypedClosure
        {
            private static readonly ClosureDelegate<TClosure>[] empty = new ClosureDelegate<TClosure>[0];

            public ClosureDelegate<TClosure>[] a = empty;
            public ClosureDelegate<TClosure>[] b = empty;
            public EventList<ClosureDelegate<TClosure>> parameterless = EventList<ClosureDelegate<TClosure>>.Create();
            public EventList<ClosureDelegate<TClosure>> parameters = EventList<ClosureDelegate<TClosure>>.Create();
            public EventListOnce<ClosureDelegate<TClosure>> parameterlessOnce = EventListOnce<ClosureDelegate<TClosure>>.Create();
            public EventListOnce<ClosureDelegate<TClosure>> parametersOnce = EventListOnce<ClosureDelegate<TClosure>>.Create();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Dispose()
            {
                parameters.Dispose();
                parameterless.Dispose();
                parametersOnce.Dispose();
                parameterlessOnce.Dispose();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Purge()
            {
                parameterless.ExtractToRun(ref a, ref b, out int count);
                parameterless.InjectToRun(ref a, count);
                parameters.ExtractToRun(ref a, ref b, out count);
                parameters.InjectToRun(ref a, count);
                parameterlessOnce.ExtractToRunRemoved(ref a, ref b, out count);
                parameterlessOnce.InjectToRun(ref a, count);
                parametersOnce.ExtractToRunRemoved(ref a, ref b, out count);
                parametersOnce.InjectToRun(ref a, count);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Raise<T>(T argument)
            {
                InnerRaise(ref parameterless, ref a, ref b, new Parameterless());
                InnerRaise(ref parameters, ref a, ref b, argument);
                InnerRaise(ref parameterlessOnce, ref a, ref b, new Parameterless());
                InnerRaise(ref parametersOnce, ref a, ref b, argument);
            }
        }
    }
}