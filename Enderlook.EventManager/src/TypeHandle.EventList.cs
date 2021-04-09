using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle
    {
        internal struct EventList<T> : IDisposable where T : IDelegate<T>
        {
            private T[] toRun;
            private int toRunCount;

            private T[] toRemove;
            private int toRemoveCount;

            public static EventList<T> Create() => new EventList<T>()
            {
                toRun = Array.Empty<T>(),
                toRemove = Array.Empty<T>(),
            };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T element) => InnerAdd(ref toRun, ref toRunCount, element);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(T element) => InnerAdd(ref toRemove, ref toRemoveCount, element);

            public void ExtractToRun(ref T[] toRunExtracted, ref T[] replacement, out int count)
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