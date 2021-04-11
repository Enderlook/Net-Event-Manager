using System;

namespace Enderlook.EventManager
{
    internal abstract class HeapClosureHandleBase<TEvent> : IDisposable
    {
        public abstract void Raise(TEvent argument);

        public abstract void Purge();

        public abstract void Dispose();

        public abstract HandleSnapshoot ExtractSnapshoot();

        public abstract void Raise(in HandleSnapshoot handleSnapshoot, TEvent argument);

        public abstract void ClearAfterRaise(in HandleSnapshoot handleSnapshoot);
    }
}