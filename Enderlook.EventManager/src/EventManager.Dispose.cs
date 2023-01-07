using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

public sealed partial class EventManager : IDisposable
{
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        // Shared state can't be disposed.
        if (ReferenceEquals(this, Shared))
            return;

        if (state == IS_DISPOSED_OR_DISPOSING)
            return;

        if (!TryMassiveWriteBegin())
            Work();

        if (state != IS_DISPOSED_OR_DISPOSING)
        {
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Work()
        {
            while (!TryMassiveWriteBegin())
            {
                int state_ = state;
                if (state_ == IS_DISPOSED_OR_DISPOSING)
                    return;
                if ((state_ & IS_PURGING) != 0)
                {
                    if ((state_ & IS_CANCELLATION_REQUESTED) != 0)
                        continue;

                    Lock(ref stateLock);
                    {
                        state_ = state;

                        if (state_ == IS_DISPOSED_OR_DISPOSING)
                        {
                            Unlock(ref stateLock);
                            return;
                        }

                        if ((state_ & IS_PURGING) != 0)
                            state = state_ | IS_CANCELLATION_REQUESTED;
                    }
                    Unlock(ref stateLock);
                }
            }
        }
    }
}
