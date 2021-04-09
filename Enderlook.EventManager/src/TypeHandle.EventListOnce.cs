﻿using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal partial class TypeHandle
    {
        internal struct EventListOnce<T> : IDisposable where T : Delegate
        {
            private T[] toRun;
            private int toRunCount;

            private T[] toRemove;
            private int toRemoveCount;

            public static EventListOnce<T> Create() => new EventListOnce<T>()
            {
                toRun = Array.Empty<T>(),
                toRemove = Array.Empty<T>(),
            };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T element) => InnerAdd(ref toRun, ref toRunCount, element);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(T element) => InnerAdd(ref toRemove, ref toRemoveCount, element);

            public void ExtractToRun(ref T[] toRunExtracted, ref T[] toRemoveExtracted, out int count, out int countRemove)
            {
                InnerSwap(ref toRun, ref toRunCount, ref toRunExtracted, out count);
                InnerSwap(ref toRemove, ref toRemoveCount, ref toRemoveExtracted, out countRemove);
            }

            public void ExtractToRunRemoved(ref T[] toRunExtracted, ref T[] replacement, out int count)
                => TypeHandle.ExtractToRun(ref toRun, ref toRemove, ref toRunCount, ref toRemoveCount, ref toRunExtracted, ref replacement, out count);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void InjectToRun(ref T[] array, int count)
                => TypeHandle.InjectToRun(ref toRun, ref toRunCount, ref array, count);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                T[] empty = Array.Empty<T>();
                T[] empty2 = empty;
                InnerSwap(ref toRun, ref toRunCount, ref empty, out int _);
                InnerSwap(ref toRemove, ref toRemoveCount, ref empty2, out int _);
            }
        }
    }
}