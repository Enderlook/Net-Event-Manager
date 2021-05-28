using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct Type2 : IEquatable<Type2>
    {
        private readonly Type eventType;
        private readonly Type closureType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type2(Type eventType, Type closureType)
        {
            this.eventType = eventType;
            this.closureType = closureType;
        }

        public bool Equals(Type2 other) => eventType.Equals(other.eventType) && closureType.Equals(other.closureType);

        public override int GetHashCode()
        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            return HashCode.Combine(eventType, closureType);
#else
            uint hash = 17;
            hash = (hash * 23) + (uint)eventType.GetHashCode();
            hash = (hash * 23) + (uint)closureType.GetHashCode();
            return (int)hash;
#endif
        }

        public override bool Equals(object obj) => obj is Type2 type2 && Equals(type2);
    }
}