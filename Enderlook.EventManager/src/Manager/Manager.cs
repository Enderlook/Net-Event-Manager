using System;

namespace Enderlook.EventManager
{
    internal abstract class Manager : IDisposable
    {
        public abstract void Dispose();

        public abstract bool Purge();
    }
}