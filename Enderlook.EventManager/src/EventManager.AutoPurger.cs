using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    private const int PurgeAttempts = 3;

    private Action<int, ParallelLoopState>? autoPurgeAction;
    private Action<InvokersHolderManager, ParallelLoopState>? autoPurgeAction2;

    private int millisecondsTimeStamp;
    private int purgingIndex;

    private bool ConcurrentPurge()
    {
        const int LowAfterMilliseconds = 180 * 1000; // Trim after 60 seconds for low pressure.
        const int MediumAfterMilliseconds = 90 * 1000; // Trim after 60 seconds for medium pressure.
        const double HighPressureThreshold = .90; // Percent of GC memory pressure threshold we consider "high".
        const double MediumPressureThreshold = .70; // Percent of GC memory pressure threshold we consider "medium".

        int currentMilliseconds = Environment.TickCount;

        for (int j = 0; j < PurgeAttempts; j++)
        {
            // The callback was called before finishing the previous purge.
            if ((state & IS_PURGING) != 0)
                return true;

            if (state == IS_DISPOSED_OR_DISPOSING)
                return false;

            MassiveWriteBegin();
            {
                if (state == IS_DISPOSED_OR_DISPOSING)
                {
                    WriteEnd();
                    return false;
                }

                if (holdersCount == 0)
                {
                    WriteEnd();
                    return true;
                }

                int trimMilliseconds;

#if NET5_0_OR_GREATER
                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();

                if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * HighPressureThreshold)
                    trimMilliseconds = 0;
                else if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * MediumPressureThreshold)
                    trimMilliseconds = MediumAfterMilliseconds;
                else
                    trimMilliseconds = LowAfterMilliseconds;
#else
                trimMilliseconds = 0;
#endif

                if (millisecondsTimeStamp == 0)
                    millisecondsTimeStamp = currentMilliseconds;

                if ((currentMilliseconds - millisecondsTimeStamp) <= trimMilliseconds)
                {
                    WriteEnd();
                    return true;
                }

                state = IS_PURGING;

                Debug.Assert(holders is not null, "Impossible state, since holders initialization happens at the same time AutoPurger is instantiated.");

                int purgingIndex_ = purgingIndex;
                if (purgingIndex_ >= 0)
                {
                    if (holdersCount > 1)
                    {
                        purgingIndex_ += (int)(Parallel.For(0, holdersCount, autoPurgeAction ??= new Action<int, ParallelLoopState>((index, loop) =>
                        {
                            if (loop.ShouldExitCurrentIteration)
                                return;

                            if ((state & IS_CANCELATION_REQUESTED) != 0)
                                loop.Stop();

                            Utils.ExpectAssignableType<InvokersHolder>(holders[(purgingIndex + index) % holdersCount]).Purge();
                        })).LowestBreakIteration ?? -1);
                    }
                    else if (holdersCount == 1)
                    {
                        Utils.ExpectAssignableType<InvokersHolder>(holders[0]).Purge();
                        purgingIndex_ = -1;
                    }
                }

                if ((state & IS_CANCELATION_REQUESTED) == 0)
                {
                    purgingIndex_ = -(purgingIndex_ + 1);

                    int holdersCountOld = holdersCount;
                    int holdersCount_ = holdersCountOld;
                    InvariantObject[] holders_ = holders;
                    // TODO: This loop could actually be done without locking the whole event manager,
                    // as long as the `holders` array is locked.

                    int i = purgingIndex_ % holdersCount_;
                    while (i < holdersCount_)
                    {
                        if ((state & IS_CANCELATION_REQUESTED) != 0)
                        {
                            purgingIndex = -(i + 1);
                            break;
                        }

                        InvokersHolder holder = Utils.ExpectAssignableType<InvokersHolder>(holders_[i].Value);
                        if (holder.RemoveIfEmpty(out Type? eventType, out InvokersHolderTypeKey holderType))
                        {
                            holdersPerType.Remove(holderType);
                            managersPerType[eventType].Remove(holder);

                            holders_[i] = holders_[--holdersCount_];
                        }
                        else
                            i++;
                    }

                    purgingIndex_ = 0;

                    if (holdersCount_ != holdersCountOld && !ArrayUtils.TryShrink(ref holders_, holdersCount_))
                        Array.Clear(holders_, holdersCount_, holdersCountOld - holdersCount_);
                    holdersCount = holdersCount_;
                    holders = holders_;

                    if ((state & IS_CANCELATION_REQUESTED) == 0)
                    {
                        // TODO: If cancelled, this has no way to reanude cleaning and must start from zero,
                        // this make vulnerable to low cleaning if purging is cancelled constantly.
                        Parallel.ForEach(managersPerType.Values, autoPurgeAction2 ??= new Action<InvokersHolderManager, ParallelLoopState>((manager, loop) =>
                        {
                            if (loop.ShouldExitCurrentIteration)
                                return;

                            if ((state & IS_CANCELATION_REQUESTED) != 0)
                                loop.Stop();

                            manager.RemoveRemovedDerived();
                        }));
                    }
                }

                purgingIndex = purgingIndex_;

                Lock(ref stateLock);
                {
                    state = 0;
                }
                Unlock(ref stateLock);

                WriteEnd();

                if ((state & IS_CANCELATION_REQUESTED) != 0 && Thread.Yield())
                    continue;

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
                    handle.Free();
            }
        }
    }
}
