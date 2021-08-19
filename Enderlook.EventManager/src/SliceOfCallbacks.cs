using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct SliceOfCallbacks
    {
        private readonly InvokersHolder holder;
        private readonly Array callbacks;
        private readonly int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SliceOfCallbacks(InvokersHolder holder, Slice slice)
        {
            this.holder = holder;
            callbacks = slice.Array;
            count = slice.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise<TEvent>(TEvent argument)
            => Utils.ExpectAssignableType<InvokersHolder<TEvent>>(holder).Raise(new(callbacks, count), argument);
    }
}
