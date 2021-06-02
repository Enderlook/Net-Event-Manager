using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct EquatableDelegate : IEquatable<EquatableDelegate>
    {
        // Delegate type is erased to avoid generic instantiation of too many ArrayPool<T>.

        public readonly Delegate callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EquatableDelegate(Delegate callback) => this.callback = callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EquatableDelegate other) => callback.Equals(other.callback);
    }
}