using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal readonly struct WeakDelegate : IWeak, IEquatable<WeakDelegate>
    {
        public readonly Delegate @delegate;
        public GCHandle Handle { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WeakDelegate(Delegate @delegate, object handle)
        {
            this.@delegate = @delegate;
            Handle = GCHandle.Alloc(handle, GCHandleType.Weak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(WeakDelegate other)
            => Handle.Target == other.Handle.Target
            && @delegate.Equals(other.@delegate);
    }
}