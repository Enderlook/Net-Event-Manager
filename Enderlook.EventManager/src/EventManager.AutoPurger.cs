using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private const int PurgeAttempts = 3;

        private Action<int, ParallelLoopState>? autoPurgeAction;
        private ParallelOptions? options;

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
                    if (holdersCount > 1)
                    {
                        purgingIndex += (int)(Parallel.For(0, holdersCount, options ??= new(), autoPurgeAction ??= new Action<int, ParallelLoopState>((index, loop) =>
                        {
                            if (loop.ShouldExitCurrentIteration)
                                return;

                            if ((state & IS_CANCELATION_REQUESTED) != 0)
                                loop.Stop();

                            Utils.ExpectAssignableType<InvokersHolder>(holders[(purgingIndex + index) % holdersCount]).Purge();
                        })).LowestBreakIteration ?? 0);
                    } else if (holdersCount == 1)
                        Utils.ExpectAssignableType<InvokersHolder>(holders[0]).Purge();

                    bool isCancelationRequested = (state & IS_CANCELATION_REQUESTED) != 0;

                    int holdersCountOld = holdersCount;
                    int holdersCount_ = holdersCountOld;
                    InvariantObject[] holders_ = holders;
                    // TODO: This loop could actually be done without locking the whole event manager,
                    // as long as the `holders` array is locked.
                    for (int i = 0; i < holdersCount_; i++)
                    {
                        InvokersHolder holder = Utils.ExpectAssignableType<InvokersHolder>(holders_[i].Value);
                        if (holder.RemoveIfEmpty())
                        {
                            holdersPerType.Remove(holder.GetType());
                            managersPerType[holder.GetEventType()].Remove(holder);

                            while (true)
                            {
                                if (--holdersCount_ == i)
                                    goto end;
                                holder = Utils.ExpectAssignableType<InvokersHolder>(holders_[holdersCount_].Value);
                                if (!holder.RemoveIfEmpty())
                                    break;
                                holdersPerType.Remove(holder.GetType());
                                managersPerType[holder.GetEventType()].Remove(holder);
                            }
                            holders_[i] = holders_[i + 1];
                        }
                    }
                    end:
                    if (!ArrayUtils.TryShrink(ref holders_, holdersCount_) && holdersCount_ != holdersCountOld)
                        Array.Clear(holders_, holdersCount_, holdersCountOld - holdersCount_);
                    holders = holders_;
                    holdersCount = holdersCount_;

                    Lock(ref stateLock);
                    {
                        state = 0;
                    }
                    Unlock(ref stateLock);

                    WriteEnd();

                    if (isCancelationRequested && Thread.Yield())
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
}