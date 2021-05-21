using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct SliceWithEventHandle
    {
        public readonly Slice slice;
        public readonly EventHandle handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SliceWithEventHandle(Slice slice, EventHandle handle)
        {
            this.slice = slice;
            this.handle = handle;
        }
    }
}