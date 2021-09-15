using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct Slice
    {
        public readonly Array Array;
        public readonly int Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slice(Array array, int count)
        {
            Array = array;
            Count = count;
        }
    }
}
