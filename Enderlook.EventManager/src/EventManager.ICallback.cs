using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    private interface ICallback
    {
#if NET7_0_OR_GREATER
        static abstract
#endif
        void Invoke(EventManager manager);
    }

    private struct Nothing : ICallback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public
#if NET7_0_OR_GREATER
        static
#endif
        void Invoke(EventManager manager)
        {
        }
    }

    private struct UnlockGlobal : ICallback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public
#if NET7_0_OR_GREATER
        static
# endif
        void Invoke(EventManager manager)
        {
            Debug.Assert(manager.spinLock.IsAcquired);
            manager.spinLock.Exit();
        }
    }
}