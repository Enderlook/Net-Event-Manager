using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct Slice
    {
        public readonly Array array;
        public readonly int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slice(Array array, int count)
        {
            this.array = array;
            this.count = count;
        }
    }
}