﻿using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventListOnce<TDelegate, TEvent> : IDisposable where TDelegate : IDelegate<TDelegate, TEvent>
    {
        private TDelegate[] toRun;
        private int toRunCount;

        private TDelegate[] toRemove;
        private int toRemoveCount;

        public static EventListOnce<TDelegate, TEvent> Create() => new EventListOnce<TDelegate, TEvent>()
        {
            toRun = Array.Empty<TDelegate>(),
            toRemove = Array.Empty<TDelegate>(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate element) => Utility.InnerAdd(ref toRun, ref toRunCount, element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate element) => Utility.InnerAdd(ref toRemove, ref toRemoveCount, element);

        public void ExtractToRun(ref TDelegate[] toRunExtracted, out int toRunCount, ref TDelegate[] toRemoveExtracted, out int toRemoveCount)
        {
            Utility.InnerSwap(ref toRun, ref this.toRunCount, ref toRunExtracted, out toRunCount);
            Utility.InnerSwap(ref toRemove, ref this.toRemoveCount, ref toRemoveExtracted, out toRemoveCount);
        }

        public void ExtractToRunRemoved(ref TDelegate[] toRunExtracted, out int toRunCount, ref TDelegate[] removedArray, out int removedArrayCount)
            => Utility.ExtractToRun<TDelegate, TEvent>(
                ref toRun, ref this.toRunCount, ref toRemove, ref toRemoveCount,
                ref toRunExtracted, out toRunCount, ref removedArray, out removedArrayCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectToRun(ref TDelegate[] array, ref int count)
            => Utility.Drain(ref toRun, ref toRunCount, ref array, ref count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            TDelegate[] empty = Array.Empty<TDelegate>();
            TDelegate[] empty2 = empty;
            Utility.InnerSwap(ref toRun, ref toRunCount, ref empty, out int _);
            Utility.InnerSwap(ref toRemove, ref toRemoveCount, ref empty2, out int _);
        }
    }
}