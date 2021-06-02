using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct DelegateWithClosure<TClosure> : IEquatable<DelegateWithClosure<TClosure>>
    {
        // Delegate type is erased to avoid generic instantiation of too many ArrayPool<T>.

        private static readonly EqualityComparer<TClosure> closureComparer = EqualityComparer<TClosure>.Default;

        public readonly Delegate callback;
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateWithClosure(Delegate callback, TClosure closure)
        {
            this.callback = callback;
            this.closure = closure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DelegateWithClosure<TClosure> other)
            => callback.Equals(other.callback) && typeof(TClosure).IsValueType ?
            EqualityComparer<TClosure>.Default.Equals(closure, other.closure) :
            closureComparer.Equals(closure, other.closure);
    }
}