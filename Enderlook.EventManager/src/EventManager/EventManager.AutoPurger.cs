using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private const int PurgeCount = 59;

        private Action<int, ParallelLoopState>? autoPurgeAction;

        private bool ConcurrentPurge()
        {
            // The callback was called befeore finishing the previous purge.
            if ((state & IS_PURGING) != 0)
                return true;

            MassiveWriteBegin();
            {
                if (state == IS_DISPOSED_OR_DISPOSING)
                {
                    WriteEnd();
                    return false;
                }

                if (managersDictionary is null)
                {
                    WriteEnd();
                    return true;
                }

#if NET5_0_OR_GREATER
                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();
                if (memoryInfo.MemoryLoadBytes < memoryInfo.HighMemoryLoadThresholdBytes * .8)
                {
                    WriteEnd();
                    return true;
                }
#endif

                state = IS_PURGING;

                if (!EnableMultithreadingForAutoCleaning)
                {
                    ValueList<object> keys1 = ValueList<object>.Create();
                    ValueList<Type2> keys2 = ValueList<Type2>.Create();
                    {
                        for (int i = 0; i <= PurgeCount; i++)
                        {
                            purgingIndex = (purgingIndex + 1) % PurgeCount;
                            if (Purge(purgingIndex, ref keys1, ref keys2))
                                break;
                        }
                    }
                    keys2.Return();
                    return Finalize(keys1);
                }
                else
                {
                    ParallelOptions options = new();
                    purgingIndex = (purgingIndex + (int)(Parallel.For(0, PurgeCount + 1, options, autoPurgeAction ??= (index, loop) =>
                    {
                        if (loop.ShouldExitCurrentIteration)
                            return;

                        if ((state & IS_CANCELATION_REQUESTED) != 0)
                            loop.Break();

                        ValueList<object> keys1 = ValueList<object>.Create();
                        ValueList<Type2> keys2 = ValueList<Type2>.Create();
                        {
                            if (Purge((purgingIndex + index) % PurgeCount, ref keys1, ref keys2))
                                loop.Stop();
                        }
                        keys1.Return();
                        keys2.Return();
                    }).LowestBreakIteration ?? 0)) % PurgeCount;
                    return Finalize(ValueList<object>.Create());
                }
            }

            bool Finalize(ValueList<object> keys)
            {
                foreach (KeyValuePair<Type, Manager> kvp in managersDictionary)
                {
                    if (kvp.Value.Purge())
                        keys.Add(kvp.Key);
                }

                for (int i = 0; i < keys.Count; i++)
                    managersDictionary.Remove(CastUtils.ExpectExactType<Type>(keys.Get(i)));

                keys.Return();

                while (Interlocked.Exchange(ref stateLock, 1) != 0) ; ;
                {
                    state = 0;
                }
                stateLock = 0;

                WriteEnd();

                return true;
            }
        }

        private bool Purge(int index, ref ValueList<object> keys1, ref ValueList<Type2> keys2)
        {
            return index switch
            {
                0 => Purge(ref multipleStrongWithArgumentHandle, ref keys1),
                1 => Purge(ref multipleStrongHandle, ref keys1),
                2 => Purge(ref multipleStrongWithArgumentWithValueClosureHandle, ref keys2),
                3 => Purge(ref multipleStrongWithArgumentWithReferenceClosureHandle, ref keys1),
                4 => Purge(ref multipleStrongWithValueClosureHandle, ref keys2),
                5 => Purge(ref multipleStrongWithReferenceClosureHandle, ref keys1),

                6 => Purge(ref onceStrongWithArgumentHandle, ref keys1),
                7 => Purge(ref onceStrongHandle, ref keys1),
                8 => Purge(ref onceStrongWithArgumentWithValueClosureHandle, ref keys2),
                9 => Purge(ref onceStrongWithArgumentWithReferenceClosureHandle, ref keys1),
                10 => Purge(ref onceStrongWithValueClosureHandle, ref keys2),
                11 => Purge(ref onceStrongWithReferenceClosureHandle, ref keys1),

                12 => Purge(ref multipleWeakWithArgumentHandle, ref keys1),
                13 => Purge(ref multipleWeakHandle, ref keys1),
                14 => Purge(ref multipleWeakWithArgumentWithValueClosureHandle, ref keys2),
                15 => Purge(ref multipleWeakWithArgumentWithReferenceClosureHandle, ref keys1),
                16 => Purge(ref multipleWeakWithValueClosureHandle, ref keys2),
                17 => Purge(ref multipleWeakWithReferenceClosureHandle, ref keys1),
                18 => Purge(ref multipleWeakWithArgumentWithValueClosureWithHandleHandle, ref keys2),
                19 => Purge(ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle, ref keys1),
                20 => Purge(ref multipleWeakWithValueClosureWithHandleHandle, ref keys2),
                21 => Purge(ref multipleWeakWithReferenceClosureWithHandleHandle, ref keys1),
                22 => Purge(ref multipleWeakWithArgumentWithHandleHandle, ref keys1),
                23 => Purge(ref multipleWeakWithHandleHandle, ref keys1),
                24 => Purge(ref multipleWeakWithArgumentHandleTrackResurrection, ref keys1),
                25 => Purge(ref multipleWeakHandleTrackResurrection, ref keys1),
                26 => Purge(ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection, ref keys2),
                27 => Purge(ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection, ref keys1),
                28 => Purge(ref multipleWeakWithValueClosureHandleTrackResurrection, ref keys2),
                29 => Purge(ref multipleWeakWithReferenceClosureHandleTrackResurrection, ref keys1),
                30 => Purge(ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection, ref keys2),
                31 => Purge(ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection, ref keys1),
                32 => Purge(ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection, ref keys2),
                33 => Purge(ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection, ref keys1),
                34 => Purge(ref multipleWeakWithArgumentWithHandleHandleTrackResurrection, ref keys1),
                35 => Purge(ref multipleWeakWithHandleHandleTrackResurrection, ref keys1),

                36 => Purge(ref onceWeakWithArgumentHandle, ref keys1),
                37 => Purge(ref onceWeakHandle, ref keys1),
                38 => Purge(ref onceWeakWithArgumentWithValueClosureHandle, ref keys2),
                39 => Purge(ref onceWeakWithArgumentWithReferenceClosureHandle, ref keys1),
                40 => Purge(ref onceWeakWithValueClosureHandle, ref keys2),
                41 => Purge(ref onceWeakWithReferenceClosureHandle, ref keys1),
                42 => Purge(ref onceWeakWithArgumentWithValueClosureWithHandleHandle, ref keys2),
                43 => Purge(ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle, ref keys1),
                44 => Purge(ref onceWeakWithValueClosureWithHandleHandle, ref keys2),
                45 => Purge(ref onceWeakWithReferenceClosureWithHandleHandle, ref keys1),
                46 => Purge(ref onceWeakWithArgumentWithHandleHandle, ref keys1),
                47 => Purge(ref onceWeakWithHandleHandle, ref keys1),
                48 => Purge(ref onceWeakWithArgumentHandleTrackResurrection, ref keys1),
                49 => Purge(ref onceWeakHandleTrackResurrection, ref keys1),
                50 => Purge(ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection, ref keys2),
                51 => Purge(ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection, ref keys1),
                52 => Purge(ref onceWeakWithValueClosureHandleTrackResurrection, ref keys2),
                53 => Purge(ref onceWeakWithReferenceClosureHandleTrackResurrection, ref keys1),
                54 => Purge(ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection, ref keys2),
                55 => Purge(ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection, ref keys1),
                56 => Purge(ref onceWeakWithValueClosureWithHandleHandleTrackResurrection, ref keys2),
                57 => Purge(ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection, ref keys1),
                58 => Purge(ref onceWeakWithArgumentWithHandleHandleTrackResurrection, ref keys1),
                59 => Purge(ref onceWeakWithHandleHandleTrackResurrection, ref keys1),

                _ => true,
            };
        }

        private sealed class AutoPurger
        {
            private readonly GCHandle handle;

            public AutoPurger(EventManager manager) => handle = GCHandle.Alloc(manager, GCHandleType.WeakTrackResurrection);

            ~AutoPurger()
            {
                object? obj = handle.Target;
                if (obj is null)
                    handle.Free();
                else
                {
                    EventManager manager = CastUtils.ExpectExactType<EventManager>(obj);
                    if (manager.ConcurrentPurge())
                        GC.ReRegisterForFinalize(this);
                }
            }
        }
    }
}