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

    private int globalLock;
    private int readers;
    private int reserved;
    private int inHolders;

    private int state;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBegin()
    {
        SpinWait spinWait = new();
        while (true)
        {
            if (TryLock(ref globalLock))
                break;
            else if (Volatile.Read(ref state) != 0)
            {
                Work(spinWait);
                break;
            }
            spinWait.SpinOnce();
        }

        if (Volatile.Read(ref state) == IS_DISPOSED_OR_DISPOSING)
            RunAndThrowObjectDisposedException<UnlockGlobal>();
        // We don't check if it's purging because that requires a global lock that we already have.
        Volatile.Write(ref readers, Volatile.Read(ref readers) + 1);
        Unlock(ref globalLock);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work(SpinWait spinWait)
        {
            while (true)
            {
                spinWait.SpinOnce();
                RequestPurgeCancellation<Nothing>(ref spinWait);
                if (TryLock(ref globalLock))
                    return;
            }
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
        Lock(ref globalLock);
        Volatile.Write(ref readers, Volatile.Read(ref readers) - 1);
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToWrite()
    {
        SpinWait spinWait = new();
        // Purging nor disposing can start while it's reading, so we ignore those checks.
        Lock(ref globalLock, ref spinWait);
        int readers_ = Volatile.Read(ref readers) - 1;
        Volatile.Write(ref readers, readers_);

        if (readers_ == 0)
            return;

        Volatile.Write(ref reserved, Volatile.Read(ref reserved) + 1);
        Unlock(ref globalLock);

        while (true)
        {
            if (TryLock(ref globalLock))
                break;
            else if (Volatile.Read(ref state) != 0)
            {
                Work(ref spinWait);
                break;
            }
            spinWait.SpinOnce();
        }

        Volatile.Write(ref reserved, Volatile.Read(ref reserved) - 1);
        if (Volatile.Read(ref state) == IS_DISPOSED_OR_DISPOSING)
            RunAndThrowObjectDisposedException<UnlockGlobal>();
        // We don't check if it's purging because that requires a global lock that we already have.
        Unlock(ref globalLock);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work(ref SpinWait spinWait)
        {
            while (true)
            {
                RequestPurgeCancellation<DecrementReserved>(ref spinWait);
                if (TryLock(ref globalLock))
                    return;
                spinWait.SpinOnce();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToInHolder()
    {
        // Purging nor disposing can't start while it's reading, so we ignore those checks.
        Lock(ref globalLock);
        {
            Volatile.Write(ref readers, Volatile.Read(ref readers) - 1);
            Interlocked.Increment(ref inHolders);
        }
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromWriteToInHolder()
    {
        Interlocked.Increment(ref inHolders);
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryMassiveWriteBegin(ref SpinWait spinWait)
    {
        while (true)
        {
            if (TryLock(ref globalLock))
            {
                if (Volatile.Read(ref readers) + Volatile.Read(ref reserved) + Volatile.Read(ref inHolders) > 0)
                    Unlock(ref globalLock);
                else
                    return true;
            }
            else if (Volatile.Read(ref state) != 0)
                return false;
            spinWait.SpinOnce();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteEnd() => Unlock(ref globalLock);

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
