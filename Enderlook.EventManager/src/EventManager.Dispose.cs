using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager : IDisposable
{
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        // Shared state can't be disposed.
        if (ReferenceEquals(this, Shared))
            return;

        if (Volatile.Read(ref state) == IS_DISPOSED_OR_DISPOSING)
            return;

        SpinWait spinWait = new();
        if (!TryMassiveWriteBegin(ref spinWait))
            Work(ref spinWait);

        if (Volatile.Read(ref state) != IS_DISPOSED_OR_DISPOSING)
        {
            Volatile.Write(ref state, IS_DISPOSED_OR_DISPOSING);

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
        void Work(ref SpinWait spinWait)
        {
            while (!TryMassiveWriteBegin(ref spinWait))
            {
                int state_ = Volatile.Read(ref state);
                while (true)
                {
                    if (state_ == IS_DISPOSED_OR_DISPOSING)
                        return;

                    if ((state_ & IS_PURGING) != 0)
                    {
                        if ((state_ & IS_CANCELLATION_REQUESTED) != 0)
                            break;

                        int oldState = Interlocked.CompareExchange(ref state, state_ | IS_CANCELLATION_REQUESTED, state_);
                        if (oldState != state_)
                        {
                            state_ = oldState;
                            spinWait.SpinOnce();
                            continue;
                        }
                    }
                    break;
                }
            }
        }
    }
}
