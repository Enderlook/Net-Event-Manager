using Enderlook.Threading.Primitives;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    private const int LOCK = 1;
    private const int UNLOCK = 0;

    private const int IS_DISPOSED_OR_DISPOSING = 1 << 1;
    private const int IS_PURGING = 1 << 2;
    private const int IS_CANCELLATION_REQUESTED = 1 << 3;

    private SpinLockSlim spinLock;

    private int inHolders;

    private int state;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Lock()
    {
        bool taken = false;
        spinLock.TryEnter(ref taken);
        if (taken)
        {
            if (Volatile.Read(ref state) == IS_DISPOSED_OR_DISPOSING)
                RunAndThrowObjectDisposedException<UnlockGlobal>();
        }
        else
            Work();

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work()
        {
            SpinWait spinWait = new();
            RequestPurgeCancellation<Nothing>(ref spinWait);
            bool taken = false;
            spinLock.Enter(ref taken, ref spinWait);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RequestPurgeCancellation<TOnDispose>(ref SpinWait spinWait)
        where TOnDispose : struct, ICallback
    {
        int state_ = Volatile.Read(ref state);
        while (true)
        {
            if (state_ == IS_DISPOSED_OR_DISPOSING)
                RunAndThrowObjectDisposedException<TOnDispose>();

            if ((state_ & IS_PURGING) != 0)
            {
                if ((state_ & IS_CANCELLATION_REQUESTED) != 0)
                    return;

                int oldState = Interlocked.CompareExchange(ref state, state_ | IS_CANCELLATION_REQUESTED, state_);
                if (oldState != state_)
                {
                    state_ = oldState;
                    spinWait.SpinOnce();
                    continue;
                }
            }
            return;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadEnd()
    {
        // Purging nor disposing can start while it's reading, so we ignore those checks.
        Debug.Assert(spinLock.IsAcquired);
        spinLock.Exit();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromLockToInHolder()
    {
        // Purging nor disposing can't start while it's reading, so we ignore those checks.
        Interlocked.Increment(ref inHolders);
        Debug.Assert(spinLock.IsAcquired);
        spinLock.Exit();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryMassiveLock(ref SpinWait spinWait)
    {
        while (true)
        {
            bool enter = false;
            spinLock.TryEnter(ref enter);
            if (enter)
            {
                if (Volatile.Read(ref inHolders) > 0)
                    spinLock.Exit();
                else
                    return true;
            }
            else if (Volatile.Read(ref state) != 0)
                return false;
            spinWait.SpinOnce();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InHolderEnd() => Interlocked.Decrement(ref inHolders);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Lock(ref int @lock)
    {
        SpinWait spinWait = new();
        while (TryLock(ref @lock))
            spinWait.SpinOnce();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Lock(ref int @lock, ref SpinWait spinWait)
    {
        while (TryLock(ref @lock))
            spinWait.SpinOnce();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryLock(ref int @lock) => Interlocked.CompareExchange(ref @lock, LOCK, UNLOCK) != UNLOCK;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Unlock(ref int @lock) => Volatile.Write(ref @lock, UNLOCK);
}
