﻿using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IDisposable
    {
        private Array<TDelegate> toRun;
        private int toRunCount;

        private Array<TDelegate> toRemove;
        private int toRemoveCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventList<TDelegate> Create()
        {
            Array<TDelegate> array = Array<TDelegate>.Empty();
            return new EventList<TDelegate>()
            {
                toRun = array,
                toRemove = array,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate element) => Utility.Add(ref toRun, ref toRunCount, element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate element) => Utility.Add(ref toRemove, ref toRemoveCount, element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExtractToRun(out Array<TDelegate> toRunExtracted, out int toRunCount)
            => Utility.ExtractToRun(ref toRun, ref this.toRunCount, ref toRemove, ref toRemoveCount, out toRunExtracted, out toRunCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectToRun(Array<TDelegate> array, int count)
            => Utility.Drain(ref toRun, ref toRunCount, array, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Utility.InjectEmpty(ref toRun, ref toRunCount);
            Utility.InjectEmpty(ref toRemove, ref toRemoveCount);
        }
    }
}