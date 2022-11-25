namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    /// <summary>
    /// Unsubscribe all listeners.
    /// </summary>
    public void Reset()
    {
        if (state != 0)
            TryRequestPurgeCancellation();

        MassiveWriteBegin();
        {
            if (state == IS_DISPOSED_OR_DISPOSING)
                ThrowObjectDisposedException();

            Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType_ = holdersPerType;
            for (int i = 0; i < holdersPerType_.EndIndex; i++)
            {
                if (holdersPerType_.TryGetFromIndex(i, out InvokersHolder holder))
                    holder.Dispose();
            }

            purgePhase = PurgePhase_PurgeInvokersHolder;
            purgeIndex = 0;
            managersPerType.Dispose();
            holdersPerType.Dispose();
        }
        WriteEnd();
    }
}
