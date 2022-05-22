using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    private const int PurgeAttempts = 3;
    private const int PurgeSleepMilliseconds = 15;

    private const int PurgePhase_FinalizerNotConfigured = 0;
    private const int PurgePhase_PurgeInvokersHolder = 1;
    private const int PurgePhase_PurgeInvokersHolderManager = 2;

    private byte purgePhase = PurgePhase_FinalizerNotConfigured;

    private int purgeIndex;

    private bool ConcurrentPurge()
    {
        const int InvokersHolder_LowAfterMilliseconds = 180 * 1000; // Trim InvokersHolder after 180 seconds for low pressure.
        const int InvokersHolder_MediumAfterMilliseconds = 90 * 1000; // Trim InvokersHolder after 90 seconds for medium pressure.

        const int InvokersHolderManager_LowAfterMilliseconds = 240 * 1000; // Trim InvokersHolder after 240 seconds for low pressure.
        const int InvokersHolderManager_MediumAfterMilliseconds = 120 * 1000; // Trim InvokersHolder after 120 seconds for medium pressure.

        Debug.Assert(purgePhase != 0);

        for (int j = 0; j < PurgeAttempts; j++)
        {
            // Check if callback was called before finishing the previous purge or if dictionaries were not used.
            if ((state & IS_PURGING) != 0 || holdersPerType.EndIndex + managersPerType.EndIndex == 0)
                return true;

            if (state == IS_DISPOSED_OR_DISPOSING)
                return false;

            MassiveWriteBegin();
            {
                if ((state & IS_PURGING) != 0 || state == IS_DISPOSED_OR_DISPOSING)
                {
                    WriteEnd();
                    return false;
                }

                int currentMilliseconds = Environment.TickCount;

                MemoryPressure memoryPressure = Utils.GetMemoryPressure();
                bool hasHighMemoryPressure = memoryPressure == MemoryPressure.High;

                state = IS_PURGING;

                int purgeIndex_ = purgeIndex;
                Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType_ = holdersPerType;
                Dictionary2<Type, InvokersHolderManager> managersPerType_ = managersPerType;

                switch (purgePhase)
                {
                    case PurgePhase_PurgeInvokersHolder:
                    {
                        int end = holdersPerType_.EndIndex;
                        int count;
                        if (purgeIndex_ >= end)
                        {
                            purgeIndex_ = 0;
                            count = end;
                        }
                        else
                            count = end - purgeIndex_;

                        int trimMilliseconds = memoryPressure switch
                        {
                            MemoryPressure.Low => InvokersHolder_LowAfterMilliseconds,
                            MemoryPressure.Medium => InvokersHolder_MediumAfterMilliseconds,
                            _ => 0,
                        };

                        for (; purgeIndex_ < end;)
                        {
                            if ((state & IS_CANCELATION_REQUESTED) == 0)
                            {
                                if (holdersPerType_.TryGetFromIndex(purgeIndex_, out InvokersHolder? holder))
                                {
                                    if (holder.Purge(out InvokersHolderTypeKey holderType, currentMilliseconds, trimMilliseconds, hasHighMemoryPressure))
                                        // Note: We could also remove this holder from the manager instead of wait to the next phase for doing that.
                                        holdersPerType_.Remove(holderType);
                                    else
                                        purgeIndex_++;

                                    // Check if we already visited all holders to break early.
                                    if (--count == 0)
                                        break;
                                }
                                else
                                    purgeIndex_++;
                            }
                            else
                                goto exit;
                        }

                        purgeIndex_ = 0;
                        holdersPerType_.TryShrink();
                        goto case PurgePhase_PurgeInvokersHolderManager;
                    }
                    case PurgePhase_PurgeInvokersHolderManager:
                    {
                        int end = managersPerType_.EndIndex;
                        int count;
                        if (purgeIndex_ >= end)
                        {
                            purgeIndex_ = 0;
                            count = end;
                        }
                        else
                            count = end - purgeIndex_;

                        int trimMilliseconds = memoryPressure switch
                        {
                            MemoryPressure.Low => InvokersHolderManager_LowAfterMilliseconds,
                            MemoryPressure.Medium => InvokersHolderManager_MediumAfterMilliseconds,
                            _ => 0,
                        };

                        for (int i = purgeIndex_; i < end;)
                        {
                            if ((state & IS_CANCELATION_REQUESTED) == 0)
                            {
                                if (managersPerType_.TryGetFromIndex(i, out InvokersHolderManager? manager))
                                {
                                    if (manager.Purge(out Type? key, currentMilliseconds, trimMilliseconds, hasHighMemoryPressure))
                                        managersPerType_.Remove(key);
                                    else
                                        i++;

                                    // Check if we already visited all holders to break early.
                                    if (--count == 0)
                                        break;
                                }
                                else
                                    i++;
                            }
                            else
                            {
                                purgePhase = PurgePhase_PurgeInvokersHolderManager;
                                purgeIndex_ = i;
                                goto exit;
                            }
                        }

                        purgeIndex_ = 0;
                        managersPerType_.TryShrink();
                        purgePhase = PurgePhase_PurgeInvokersHolder;
                        break;
                    }
                }

            exit:
                purgeIndex = purgeIndex_;
                holdersPerType = holdersPerType_;
                managersPerType = managersPerType_;

                Lock(ref stateLock);
                {
                    state = 0;
                }
                Unlock(ref stateLock);

                WriteEnd();

                if ((state & IS_CANCELATION_REQUESTED) != 0)
                {
                    if (!Thread.Yield())
                        Thread.Sleep(PurgeSleepMilliseconds);
                    continue;
                }

                return true;
            }
        }

        Debug.Fail("Impossible state.");

        return true;
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
                EventManager manager = Utils.ExpectExactType<EventManager>(obj);
                if (manager.ConcurrentPurge())
                    GC.ReRegisterForFinalize(this);
                else
                {
                    manager.Dispose();
                    handle.Free();
                }
            }
        }
    }
}
