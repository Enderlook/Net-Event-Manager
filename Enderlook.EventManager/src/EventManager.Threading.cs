using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    private const int LOCK = 1;
    private const int UNLOCK = 0;

    private const int IS_DISPOSED_OR_DISPOSING = 1 << 1;
    private const int IS_PURGING = 1 << 2;
    private const int IS_CANCELATION_REQUESTED = 1 << 3;

    private int globalLock;
    private int readers;
    private int reserved;
    private int inHolders;

    private int stateLock;
    private int state;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBegin()
    {
        if (state != 0)
            TryRequestPurgeCancellation();

        Lock(ref globalLock);
        {
            if (state == IS_DISPOSED_OR_DISPOSING)
                ThrowObjectDisposedException();

            readers++;
        }
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void TryRequestPurgeCancellation()
    {
        int state = this.state;

        if (state == IS_DISPOSED_OR_DISPOSING)
            ThrowObjectDisposedException();

        if ((state & IS_PURGING) != 0)
        {
            Lock(ref stateLock);
            {
                state = this.state;

                if (state == IS_DISPOSED_OR_DISPOSING)
                {
                    Unlock(ref stateLock);
                    ThrowObjectDisposedException();
                }

                if ((state & IS_PURGING) != 0)
                    this.state |= IS_CANCELATION_REQUESTED;
            }
            Unlock(ref stateLock);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadEnd()
    {
        Lock(ref globalLock);
        readers--;
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToWrite()
    {
        Lock(ref globalLock);
        readers--;

        if (readers == 0)
            return;

        reserved++;
        Unlock(ref globalLock);

        while (true)
        {
            Lock(ref globalLock);
            if (readers > 0)
            {
                reserved--;
                Unlock(ref globalLock);
            }
            else
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromReadToInHolder()
    {
        Lock(ref globalLock);
        readers--;
        Interlocked.Increment(ref inHolders);
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FromWriteToInHolder()
    {
        Interlocked.Increment(ref inHolders);
        Unlock(ref globalLock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MassiveWriteBegin()
    {
        while (true)
        {
            Lock(ref globalLock);
            if (readers + reserved + inHolders > 0)
                Unlock(ref globalLock);
            else
                break;
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
    private static void Unlock(ref int @lock) => @lock = UNLOCK;
}
