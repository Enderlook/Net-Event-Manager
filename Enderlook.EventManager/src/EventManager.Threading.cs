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

    private int globalLock;
    private int readers;
    private int reserved;
    private int inHolders;

    private int stateLock;
    private int state;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBegin()
    {
        while (true)
        {
            if (TryLock(ref globalLock))
                break;
            else if (state != 0)
            {
                Work();
                break;
            }
        }

        if (state == IS_DISPOSED_OR_DISPOSING)
            ThrowObjectDisposedExceptionAndUnlockGlobal();
        // We don't check if it's purging because that requires a global lock that we already have.
        readers++;
        Unlock(ref globalLock);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work()
        {
            while (true)
            {
                RequestPurgeCancellation();
                if (TryLock(ref globalLock))
                    return;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RequestPurgeCancellation()
    {
        int state_ = state;

        if (state_ == IS_DISPOSED_OR_DISPOSING)
            ThrowObjectDisposedException();

        if ((state_ & IS_PURGING) != 0)
        {
            if ((state_ & IS_CANCELLATION_REQUESTED) != 0)
                return;

            Lock(ref stateLock);
            {
                state_ = state;

                if (state_ == IS_DISPOSED_OR_DISPOSING)
                {
                    Unlock(ref stateLock);
                    ThrowObjectDisposedException();
                }

                if ((state_ & IS_PURGING) != 0)
                    state = state_ | IS_CANCELLATION_REQUESTED;
            }
            Unlock(ref stateLock);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadEnd()
    {
        // Purging nor disposing can start while it's reading, so we ignore those checks.
        Lock(ref globalLock);
        readers--;
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToWrite()
    {
        // Purging nor disposing can start while it's reading, so we ignore those checks.
        Lock(ref globalLock);
        int readers_ = --readers;

        if (readers_ == 0)
            return;

        reserved++;
        Unlock(ref globalLock);

        while (true)
        {
            if (TryLock(ref globalLock))
                break;
            else if (state != 0)
            {
                Work();
                break;
            }
        }

        reserved--;
        if (state == IS_DISPOSED_OR_DISPOSING)
            ThrowObjectDisposedExceptionAndUnlockGlobal();
        // We don't check if it's purging because that requires a global lock that we already have.
        Unlock(ref globalLock);

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work()
        {
            while (true)
            {
                RequestPurgeCancellation();
                if (TryLock(ref globalLock))
                    return;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToInHolder()
    {
        // Purging nor disposing can start while it's reading, so we ignore those checks.
        Lock(ref globalLock);
        {
            readers--;
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
    private bool TryMassiveWriteBegin()
    {
        while (true)
        {
            if (TryLock(ref globalLock))
            {
                if (readers + reserved + inHolders > 0)
                    Unlock(ref globalLock);
                else
                    return true;
            }
            else if (state != 0)
                return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteEnd() => Unlock(ref globalLock);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InHolderEnd() => Interlocked.Decrement(ref inHolders);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Lock(ref int @lock)
    {
        while (Interlocked.Exchange(ref @lock, LOCK) != UNLOCK) ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryLock(ref int @lock) => Interlocked.Exchange(ref @lock, LOCK) != UNLOCK;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Unlock(ref int @lock) => @lock = UNLOCK;
}
