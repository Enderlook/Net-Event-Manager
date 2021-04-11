using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct ClosureDelegate<TClosure>
    {
        public readonly Delegate @delegate;
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ClosureDelegate(Delegate @delegate, TClosure closure)
        {
            this.@delegate = @delegate;
            this.closure = closure;
        }
    }
}