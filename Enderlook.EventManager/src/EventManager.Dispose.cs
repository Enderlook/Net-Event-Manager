using System;

namespace Enderlook.EventManager;

public sealed partial class EventManager : IDisposable
{
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (state == IS_DISPOSED_OR_DISPOSING)
            return;

        MassiveWriteBegin();
        {
            if (state == IS_DISPOSED_OR_DISPOSING)
            {
                WriteEnd();
                return;
            }

            Lock(ref stateLock);
            {
                state = IS_DISPOSED_OR_DISPOSING;
            }
            Unlock(ref stateLock);

            Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType_ = holdersPerType;
            Dictionary2<Type, InvokersHolderManager> managersPerType_ = managersPerType;

            holdersPerType = new();
            managersPerType = new();

            int i = 0;
            while (holdersPerType_.MoveNext(ref i, out InvokersHolder? holder))
                holder.Dispose();

            i = 0;
            while (managersPerType_.MoveNext(ref i, out InvokersHolderManager? holder))
                holder.Dispose();
        }
        WriteEnd();
    }
}
