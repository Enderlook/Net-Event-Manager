using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private bool ConcurrentPurge()
        {
            globalLock.WriteBegin();
            {
                if (isDisposedOrDisposing)
                {
                    globalLock.ReadEnd();
                    return false;
                }

#if NETSTANDARD_2_1
                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();
                if (memoryInfo.MemoryLoadBytes < memoryInfo.HighMemoryLoadThresholdBytes * .8)
                {
                    globalLock.WriteEnd();
                    return true;
                }
#endif

                ValueList<Type> keys = ValueList<Type>.Create();
                {
                    multipleStrongWithArgumentHandle.Purge(ref keys);
                    multipleStrongHandle.Purge(ref keys);
                    multipleStrongWithArgumentWithReferenceClosureHandle.Purge(ref keys);
                    multipleStrongWithReferenceClosureHandle.Purge(ref keys);
                    onceStrongWithArgumentHandle.Purge(ref keys);
                    onceStrongHandle.Purge(ref keys);
                    onceStrongWithArgumentWithReferenceClosureHandle.Purge(ref keys);
                    onceStrongWithReferenceClosureHandle.Purge(ref keys);

                    multipleWeakWithArgumentHandle.Purge(ref keys);
                    multipleWeakHandle.Purge(ref keys);
                    multipleWeakWithArgumentWithValueClosureHandle = default;
                    multipleWeakWithArgumentWithReferenceClosureHandle.Purge(ref keys);
                    multipleWeakWithReferenceClosureHandle.Purge(ref keys);
                    multipleWeakWithArgumentWithReferenceClosureWithHandleHandle.Purge(ref keys);
                    multipleWeakWithReferenceClosureWithHandleHandle.Purge(ref keys);
                    multipleWeakWithArgumentWithHandleHandle.Purge(ref keys);
                    multipleWeakWithHandleHandle.Purge(ref keys);
                    multipleWeakWithArgumentHandleTrackResurrection.Purge(ref keys);
                    multipleWeakHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithReferenceClosureHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithArgumentWithHandleHandleTrackResurrection.Purge(ref keys);
                    multipleWeakWithHandleHandleTrackResurrection.Purge(ref keys);

                    onceWeakWithArgumentHandle.Purge(ref keys);
                    onceWeakHandle.Purge(ref keys);
                    onceWeakWithArgumentWithValueClosureHandle = default;
                    onceWeakWithArgumentWithReferenceClosureHandle.Purge(ref keys);
                    onceWeakWithReferenceClosureHandle.Purge(ref keys);
                    onceWeakWithArgumentWithReferenceClosureWithHandleHandle.Purge(ref keys);
                    onceWeakWithReferenceClosureWithHandleHandle.Purge(ref keys);
                    onceWeakWithArgumentWithHandleHandle.Purge(ref keys);
                    onceWeakWithHandleHandle.Purge(ref keys);
                    onceWeakWithArgumentHandleTrackResurrection.Purge(ref keys);
                    onceWeakHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithReferenceClosureHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithReferenceClosureWithHandleHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithArgumentWithHandleHandleTrackResurrection.Purge(ref keys);
                    onceWeakWithHandleHandleTrackResurrection.Purge(ref keys);

                    ValueList<Type2> keys2 = ValueList<Type2>.Create();
                    {
                        multipleStrongWithArgumentWithValueClosureHandle.Purge(ref keys2);
                        multipleStrongWithValueClosureHandle.Purge(ref keys2);
                        onceStrongWithArgumentWithValueClosureHandle.Purge(ref keys2);
                        onceStrongWithValueClosureHandle.Purge(ref keys2);

                        multipleWeakWithValueClosureHandle.Purge(ref keys2);
                        multipleWeakWithArgumentWithValueClosureWithHandleHandle.Purge(ref keys2);
                        multipleWeakWithArgumentWithValueClosureHandleTrackResurrection.Purge(ref keys2);
                        multipleWeakWithValueClosureWithHandleHandle.Purge(ref keys2);
                        multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection.Purge(ref keys2);
                        multipleWeakWithValueClosureHandleTrackResurrection.Purge(ref keys2);
                        multipleWeakWithValueClosureWithHandleHandleTrackResurrection.Purge(ref keys2);

                        onceWeakWithValueClosureHandle.Purge(ref keys2);
                        onceWeakWithArgumentWithValueClosureWithHandleHandle.Purge(ref keys2);
                        onceWeakWithArgumentWithValueClosureHandleTrackResurrection.Purge(ref keys2);
                        onceWeakWithValueClosureWithHandleHandle.Purge(ref keys2);
                        onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection.Purge(ref keys2);
                        onceWeakWithValueClosureHandleTrackResurrection.Purge(ref keys2);
                        onceWeakWithValueClosureWithHandleHandleTrackResurrection.Purge(ref keys2);
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
            globalLock.WriteEnd();
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