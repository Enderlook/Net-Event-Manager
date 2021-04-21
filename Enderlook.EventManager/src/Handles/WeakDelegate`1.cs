using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal readonly struct WeakDelegate<TClosure> : IWeak, IEquatable<WeakDelegate<TClosure>>
    {
        public readonly Delegate @delegate;
        public GCHandle Handle { get; }
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WeakDelegate(Delegate @delegate, object handle, TClosure closure)
        {
            this.@delegate = @delegate;
            Handle = GCHandle.Alloc(handle, GCHandleType.Weak);
            this.closure = closure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(WeakDelegate<TClosure> other)
            => Handle.Target == other.Handle.Target
            && @delegate.Equals(other.@delegate)
            && EqualityComparer<TClosure>.Default.Equals(closure, other.closure);
    }
}