using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private bool ConcurrentPurge()
        {
            MassiveWriteBegin();
            {
                if (isDisposedOrDisposing)
                {
                    ReadEnd();
                    return false;
                }

#if NETSTANDARD_2_1
                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();
                if (memoryInfo.MemoryLoadBytes < memoryInfo.HighMemoryLoadThresholdBytes * .8)
                {
                    WriteEnd();
                    return true;
                }
#endif

                ValueList<Type> keys = ValueList<Type>.Create();
                {
                    Purge(ref multipleStrongWithArgumentHandle, ref keys);
                    Purge(ref multipleStrongHandle, ref keys);
                    Purge(ref multipleStrongWithArgumentWithReferenceClosureHandle, ref keys);
                    Purge(ref multipleStrongWithReferenceClosureHandle, ref keys);
                    Purge(ref onceStrongWithArgumentHandle, ref keys);
                    Purge(ref onceStrongHandle, ref keys);
                    Purge(ref onceStrongWithArgumentWithReferenceClosureHandle, ref keys);
                    Purge(ref onceStrongWithReferenceClosureHandle, ref keys);

                    Purge(ref multipleWeakWithArgumentHandle, ref keys);
                    Purge(ref multipleWeakHandle, ref keys);
                    Purge(ref multipleWeakWithArgumentWithReferenceClosureHandle, ref keys);
                    Purge(ref multipleWeakWithReferenceClosureHandle, ref keys);
                    Purge(ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle, ref keys);
                    Purge(ref multipleWeakWithReferenceClosureWithHandleHandle, ref keys);
                    Purge(ref multipleWeakWithArgumentWithHandleHandle, ref keys);
                    Purge(ref multipleWeakWithHandleHandle, ref keys);
                    Purge(ref multipleWeakWithArgumentHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithReferenceClosureHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithArgumentWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref multipleWeakWithHandleHandleTrackResurrection, ref keys);

                    Purge(ref onceWeakWithArgumentHandle, ref keys);
                    Purge(ref onceWeakHandle, ref keys);
                    Purge(ref onceWeakWithArgumentWithReferenceClosureHandle, ref keys);
                    Purge(ref onceWeakWithReferenceClosureHandle, ref keys);
                    Purge(ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle, ref keys);
                    Purge(ref onceWeakWithReferenceClosureWithHandleHandle, ref keys);
                    Purge(ref onceWeakWithArgumentWithHandleHandle, ref keys);
                    Purge(ref onceWeakWithHandleHandle, ref keys);
                    Purge(ref onceWeakWithArgumentHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithReferenceClosureHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithArgumentWithHandleHandleTrackResurrection, ref keys);
                    Purge(ref onceWeakWithHandleHandleTrackResurrection, ref keys);

                    ValueList<Type2> keys2 = ValueList<Type2>.Create();
                    {
                        Purge(ref multipleStrongWithArgumentWithValueClosureHandle, ref keys2);
                        Purge(ref multipleStrongWithValueClosureHandle, ref keys2);
                        Purge(ref onceStrongWithArgumentWithValueClosureHandle, ref keys2);
                        Purge(ref onceStrongWithValueClosureHandle, ref keys2);

                        Purge(ref multipleWeakWithValueClosureHandle, ref keys2);
                        Purge(ref multipleWeakWithArgumentWithValueClosureWithHandleHandle, ref keys2);
                        Purge(ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection, ref keys2);
                        Purge(ref multipleWeakWithValueClosureWithHandleHandle, ref keys2);
                        Purge(ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection, ref keys2);
                        Purge(ref multipleWeakWithValueClosureHandleTrackResurrection, ref keys2);
                        Purge(ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection, ref keys2);
                        Purge(ref multipleWeakWithArgumentWithValueClosureHandle, ref keys2);

                        Purge(ref onceWeakWithValueClosureHandle, ref keys2);
                        Purge(ref onceWeakWithArgumentWithValueClosureWithHandleHandle, ref keys2);
                        Purge(ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection, ref keys2);
                        Purge(ref onceWeakWithValueClosureWithHandleHandle, ref keys2);
                        Purge(ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection, ref keys2);
                        Purge(ref onceWeakWithValueClosureHandleTrackResurrection, ref keys2);
                        Purge(ref onceWeakWithValueClosureWithHandleHandleTrackResurrection, ref keys2);
                        Purge(ref onceWeakWithArgumentWithValueClosureHandle, ref keys2);
                    }
                    keys2.Return();

                    foreach (KeyValuePair<Type, Manager> kvp in managersDictionary)
                    {
                        if (kvp.Value.Purge())
                            keys.Add(kvp.Key);
                    }

                    for (int i = 0; i < keys.Count; i++)
                        managersDictionary.Remove(keys.Get(i));
                }
                keys.Return();
            }
            WriteEnd();
            return true;
        }

        private sealed class AutoPurger
        {
            private readonly GCHandle handle;

            public AutoPurger(EventManager manager) => handle = GCHandle.Alloc(manager, GCHandleType.WeakTrackResurrection);

            ~AutoPurger()
            {
                object obj = handle.Target;
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