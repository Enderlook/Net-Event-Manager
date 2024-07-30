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
            SpinWait spinWait = new();
            int state_;
            while (!TryMassiveLock(ref spinWait))
            {
                state_ = Volatile.Read(ref state);
                if ((state_ & IS_PURGING) != 0)
                    return true;
                if (state_ == IS_DISPOSED_OR_DISPOSING)
                    return false;
                spinWait.SpinOnce();
            }

            state_ = Volatile.Read(ref state);
            // Check if callback was called before finishing the previous purge or if dictionaries were not used.
            if ((state_ & IS_PURGING) != 0 || holdersPerType.EndIndex + managersPerType.EndIndex == 0)
                return true;

            if (state_ == IS_DISPOSED_OR_DISPOSING)
                return false;

            // Check if is already purging or disposing, or is disposed.
            // If neither, set as purging.
            while (true)
            {
                if ((state_ & IS_PURGING) != 0)
                {
                    spinLock.Exit();
                    return true;
                }
                if (state_ == IS_DISPOSED_OR_DISPOSING)
                {
                    spinLock.Exit();
                    return false;
                }
                int oldState = Interlocked.CompareExchange(ref state, IS_PURGING, state_);
                if (oldState != state_)
                {
                    state_ = oldState;
                    spinWait.SpinOnce();
                    continue;
                }
                break;
            }

            Purge();

            state_ = Interlocked.Exchange(ref state, 0);

            spinLock.Exit();

            if ((state_ & IS_CANCELLATION_REQUESTED) != 0)
            {
                if (!Thread.Yield())
                    Thread.Sleep(PurgeSleepMilliseconds);
                continue;
            }

            return true;
        }

        Debug.Fail("Impossible state.");

        return true;

        void Purge()
        {
            int currentMilliseconds = Environment.TickCount;

            MemoryPressure memoryPressure = Utils.GetMemoryPressure();
            bool hasHighMemoryPressure = memoryPressure == MemoryPressure.High;

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

                    for (; count > 0; count--)
                    {
                        if ((Volatile.Read(ref state) & IS_CANCELLATION_REQUESTED) == 0)
                        {
                            if (holdersPerType_.TryGetFromIndex(purgeIndex_, out InvokersHolder? holder)
                                && holder.Purge(out InvokersHolderTypeKey holderType, currentMilliseconds, trimMilliseconds, hasHighMemoryPressure))
                                // Note: We could also remove this holder from the manager instead of wait to the next phase for doing that.
                                holdersPerType_.Remove(holderType);
                            purgeIndex_++;
                            if (purgeIndex_ == end)
                                purgeIndex_ = 0;
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

                    for (; count > 0; count--)
                    {
                        if ((Volatile.Read(ref state) & IS_CANCELLATION_REQUESTED) == 0)
                        {
                            if (managersPerType_.TryGetFromIndex(purgeIndex_, out InvokersHolderManager? manager)
                                && manager.Purge(out Type? key, currentMilliseconds, trimMilliseconds, hasHighMemoryPressure))
                                managersPerType_.Remove(key);
                            purgeIndex_++;
                            if (purgeIndex_ == end)
                                purgeIndex_ = 0;
                        }
                        else
                        {
                            purgePhase = PurgePhase_PurgeInvokersHolderManager;
                            goto exit;
                        }
                    }

                    purgeIndex_ = 0;
                    managersPerType_.TryShrink();
                    purgePhase = PurgePhase_PurgeInvokersHolder;
                    break;
                }
            }

        exit:;
            purgeIndex = purgeIndex_;
            holdersPerType = holdersPerType_;
            managersPerType = managersPerType_;
        }
    }

    private sealed class AutoPurger
    {
        private readonly GCHandle handle;

        static AutoPurger()
        {
            // We use this static constructor instead of EventManager's static constructor to prevent adding an static constructor in a public type.
            StaticAutoPurger _ = new();
        }

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

    private sealed class StaticAutoPurger
    {
        ~StaticAutoPurger()
        {
            const int LowAfterMilliseconds = 240 * 1000; // Trim after 240 seconds for low pressure.
            const int MediumAfterMilliseconds = 120 * 1000; // Trim after 120 seconds for medium pressure.

            SpinWait spinWait = new();
            for (int j = 0; j < PurgeAttempts; j++)
            {
                // Check if dictionary was not used.
                if (invokersHolderManagerCreators.EndIndex == 0)
                    return;

                bool taken = false;
                invokersHolderManagerCreatorsLock.Enter(ref taken, ref spinWait);

                taken = false;
                invokersHolderManagerCreatorsStateLock.Enter(ref taken, ref spinWait);

                if (invokersHolderManagerCreatorsState == IS_CANCELLATION_REQUESTED)
                {
                    invokersHolderManagerCreatorsStateLock.Exit();
                    goto exit;
                }
                invokersHolderManagerCreatorsState = IS_PURGING;

                int currentMilliseconds = Environment.TickCount;
                MemoryPressure memoryPressure = Utils.GetMemoryPressure();
                int trimMilliseconds = memoryPressure switch
                {
                    MemoryPressure.Low => LowAfterMilliseconds,
                    MemoryPressure.Medium => MediumAfterMilliseconds,
                    _ => 0,
                };

                int purgeIndex_ = invokersHolderManagerCreatorsPurgeIndex;
                int end = invokersHolderManagerCreators.EndIndex;
                int count;
                if (purgeIndex_ >= end)
                {
                    purgeIndex_ = 0;
                    count = end;
                }
                else
                    count = end - purgeIndex_;

                for (; count > 0; count--)
                {
                    if ((invokersHolderManagerCreatorsState & IS_CANCELLATION_REQUESTED) == 0)
                    {
                        if (invokersHolderManagerCreators.TryGetFromIndex(purgeIndex_, out Type? key, out (Func<EventManager, InvokersHolderManager>? Delegate, int MillisecondsTimestamp) value)
                            && (currentMilliseconds - value.MillisecondsTimestamp) > trimMilliseconds)
                        {
                            invokersHolderManagerCreators.Remove(key);
                            continue;
                        }
                        purgeIndex_++;
                        if (purgeIndex_ == end)
                            purgeIndex_ = 0;
                    }
                    else
                        break;
                }

                if ((invokersHolderManagerCreatorsState & IS_CANCELLATION_REQUESTED) == 0)
                    invokersHolderManagerCreators.TryShrink();

                invokersHolderManagerCreatorsPurgeIndex = purgeIndex_;

            exit:
                invokersHolderManagerCreatorsLock.Exit();

                if ((invokersHolderManagerCreatorsState & IS_CANCELLATION_REQUESTED) == 0)
                {
                    invokersHolderManagerCreatorsState = 0;
                    break;
                }

                if (!Thread.Yield())
                    Thread.Sleep(PurgeSleepMilliseconds);
            }

            GC.ReRegisterForFinalize(this);
        }
    }
}
