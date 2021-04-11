using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class HeapClosureHandle<TClosure, TEvent> : HeapClosureHandleBase<TEvent>
    {
        public ClosureHandle<TClosure, TEvent> handle = ClosureHandle<TClosure, TEvent>.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Raise(TEvent argument) => handle.Raise(argument);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Purge() => handle.Purge();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Dispose() => handle.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override HandleSnapshoot ExtractSnapshoot() => handle.ExtractSnapshoot();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Raise(in HandleSnapshoot handleSnapshoot, TEvent argument) => handle.Raise(handleSnapshoot, argument);
    }
}