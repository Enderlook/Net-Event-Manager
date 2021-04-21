using System;

namespace Enderlook.EventManager
{
    internal abstract class Handle : IDisposable
    {
        public abstract void Compact();

        public abstract void CompactAndPurge();

        public abstract void Dispose();
    }
}