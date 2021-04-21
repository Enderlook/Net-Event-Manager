using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class GlobalHandle<TEvent> : Handle
    {
        private List<RaisableHandle<TEvent>> list = List<RaisableHandle<TEvent>>.Empty();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(RaisableHandle<TEvent> handle) => list.ConcurrentAdd(handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(TEvent argument)
        {
            List<RaisableHandle<TEvent>> stolen = List<RaisableHandle<TEvent>>.Steal(ref list);
            List<(Array, int)> snapshoots = List<(Array, int)>.Rent(stolen.Count);
            for (int i = 0; i < stolen.Count; i++)
            {
                stolen[i].GetSnapshoot(out Array array, out int count);
                snapshoots.Add((array, count));
            }
            List<RaisableHandle<TEvent>>.Overwrite(ref list, stolen);

            for (int i = 0; i < stolen.Count; i++)
            {
                (Array array, int count) snapshoot = snapshoots[i];
                list.ConcurrentGet(i).Raise(snapshoot.array, snapshoot.count, argument);
            }
        }

        public override void Compact()
        {
            for (int i = 0; i < list.Count; i++)
                list.ConcurrentGet(i).Compact();
        }

        public override void CompactAndPurge()
        {
            for(int i = 0; i < list.Count; i++)
                list.ConcurrentGet(i).Compact();
        }

        public override void Dispose()
        {
            for (int i = 0; i < list.Count; i++)
                list.ConcurrentGet(i).Dispose();
        }
    }
}