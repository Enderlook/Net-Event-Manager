namespace Enderlook.EventManager
{
    internal abstract class TypedEventHandle<TEvent> : EventHandle
    {
        public abstract bool IsEmpty { get; }

        public abstract SliceWithEventHandle ConcurrentGetRaiser();

        public abstract void ConcurrentRaise(Slice slice, TEvent argument);
    }
}