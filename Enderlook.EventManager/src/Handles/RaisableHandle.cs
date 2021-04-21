using System;

namespace Enderlook.EventManager
{
    internal abstract class RaisableHandle<TEvent> : Handle
    {
        public abstract void Raise(Array obj, int count, TEvent argument);

        public abstract void GetSnapshoot(out Array obj, out int count);
    }
}