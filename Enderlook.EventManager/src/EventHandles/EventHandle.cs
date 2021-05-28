using System;

namespace Enderlook.EventManager
{
    internal abstract class EventHandle : IDisposable
    {
        public abstract void Dispose();

        public abstract bool Purge();
    }
}