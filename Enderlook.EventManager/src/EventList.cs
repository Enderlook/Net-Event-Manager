using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate, TEvent> : IDisposable
    {
        private TDelegate[] toRun;
        private int toRunCount;

        private TDelegate[] toRemove;
        private int toRemoveCount;

        public static EventList<TDelegate, TEvent> Create() => new EventList<TDelegate, TEvent>()
        {
            toRun = Array.Empty<TDelegate>(),
            toRemove = Array.Empty<TDelegate>(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate element) => Utility.InnerAdd(ref toRun, ref toRunCount, element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate element) => Utility.InnerAdd(ref toRemove, ref toRemoveCount, element);

        public void ExtractToRun(ref TDelegate[] toRunExtracted, out int toRunCount, ref TDelegate[] removedArray, out int removedArrayCount)
            => Utility.ExtractToRun(ref toRun, ref this.toRunCount, ref toRemove, ref toRemoveCount,
                                    ref toRunExtracted, out toRunCount, ref removedArray, out removedArrayCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectToRun(ref TDelegate[] array, ref int count)
            => Utility.Drain(ref toRun, ref toRunCount, ref array, ref count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            TDelegate[] empty = Utility.Container<TDelegate>.empty;
            TDelegate[] empty2 = empty;
            Utility.InnerSwap(ref toRun, ref toRunCount, ref empty, out int _);
            Utility.InnerSwap(ref toRemove, ref toRemoveCount, ref empty2, out int _);
        }
    }
}