using System;
using System.Threading.Tasks;

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

            GC.SuppressFinalize(this);

            Lock(ref stateLock);
            {
                state = IS_DISPOSED_OR_DISPOSING;
            }
            Unlock(ref stateLock);

            holdersPerType = null!;
            managersPerType = null!;
            autoPurgeAction = null;
            if (holders is not null)
            {
                if (holdersCount != 0)
                {
                    if (holdersCount == 1)
                        Utils.ExpectAssignableType<InvokersHolder>(holders[0]).Dispose();
                    else
                        Parallel.For(0, holdersCount, i => Utils.ExpectAssignableType<InvokersHolder>(holders[i]).Dispose());
                }
                holders = null!;
            }
        }
        WriteEnd();
    }
}
