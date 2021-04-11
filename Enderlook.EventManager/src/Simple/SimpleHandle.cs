using System;

namespace Enderlook.EventManager
{
    internal abstract class SimpleHandle : IDisposable
    {
        public abstract void Dispose();

        public abstract void Purge();
    }
}