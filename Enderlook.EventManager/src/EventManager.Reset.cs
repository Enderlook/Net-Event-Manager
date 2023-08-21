using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    /// <summary>
    /// Unsubscribe all listeners.
    /// </summary>
    public void Reset()
    {
        SpinWait spinWait = new();
        if (!TryMassiveWriteBegin(ref spinWait))
            Work(ref spinWait);

        if (Volatile.Read(ref state) == IS_DISPOSED_OR_DISPOSING)
            RunAndThrowObjectDisposedException<UnlockGlobal>();

        Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType_ = holdersPerType;
        for (int i = 0; i < holdersPerType_.EndIndex; i++)
        {
            if (holdersPerType_.TryGetFromIndex(i, out InvokersHolder? holder))
                holder.Dispose();
        }

        purgePhase = PurgePhase_PurgeInvokersHolder;
        purgeIndex = 0;
        managersPerType.Dispose();
        holdersPerType.Dispose();

        WriteEnd();

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work(ref SpinWait spinWait)
        {
            while (true)
            {
                RequestPurgeCancellation<Nothing>(ref spinWait);
                if (TryMassiveWriteBegin(ref spinWait))
                    return;
            }
        }
    }
}
