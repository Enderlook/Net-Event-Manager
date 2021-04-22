using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private sealed class AutoPurger
        {
            // TODO: This purger doesn't remove empty handles which means it can leak if there are too many unused types.

            private readonly GCHandle handle;

            public AutoPurger(EventManager manager) => handle = GCHandle.Alloc(manager, GCHandleType.Weak);

            ~AutoPurger()
            {
                object target = handle.Target;
                if (target is null)
                {
                    handle.Free();
                    return;
                }
                Debug.Assert(target is EventManager);
                Unsafe.As<EventManager>(target).Purge();
                GC.ReRegisterForFinalize(this);
            }
        }
    }
}