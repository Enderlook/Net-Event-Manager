using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

            int holdersCount = this.holdersCount;
            if (holdersCount != 0)
            {
                InvariantObject[]? holders = this.holders;
                Debug.Assert(holders is not null);

                holdersPerType.Clear();
                managersPerType.Clear();
                purgingIndex = 0;
                millisecondsTimeStamp = 0;

                if (holdersCount == 1)
                    Utils.ExpectAssignableType<InvokersHolder>(holders[0].Value).Dispose();
                else
                    Parallel.For(0, holdersCount, i => Utils.ExpectAssignableType<InvokersHolder>(holders[i].Value).Dispose());

                Array.Clear(holders, 0, holdersCount);
            }
        }
        WriteEnd();
    }
}
