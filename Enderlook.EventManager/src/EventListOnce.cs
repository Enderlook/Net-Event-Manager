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

        public void ExtractToRun(ref TDelegate[] toRunExtracted, ref TDelegate[] toRemoveExtracted, out int count, out int countRemove)
        {
            Utility.InnerSwap(ref toRun, ref toRunCount, ref toRunExtracted, out count);
            Utility.InnerSwap(ref toRemove, ref toRemoveCount, ref toRemoveExtracted, out countRemove);
        }

        public void ExtractToRunRemoved(ref TDelegate[] toRunExtracted, ref TDelegate[] replacement, out int count)
            => Utility.ExtractToRun<TDelegate, TEvent>(ref toRun, ref toRemove, ref toRunCount, ref toRemoveCount, ref toRunExtracted, ref replacement, out count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectToRun(ref TDelegate[] array, int count)
            => Utility.InjectToRun(ref toRun, ref toRunCount, ref array, count);

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