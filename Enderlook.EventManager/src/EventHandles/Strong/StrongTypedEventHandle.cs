using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class StrongTypedEventHandle<TEvent, TElement> : TypedEventHandle<TEvent>
    {
        protected ValueList<TElement> list = ValueList<TElement>.Create();

        public sealed override bool IsEmpty {
            get {
                Debug.Assert(!list.IsLocked);
                return list.Count > 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Add(TElement callback) => list.ConcurrentAdd(callback);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Remove(TElement callback) => list.ConcurrentRemove(callback);

        public sealed override bool Purge()
        {
            Debug.Assert(!list.IsLocked);
            if (list.Count == 0)
            {
                list.Return();
                list = ValueList<TElement>.Create();
                return true;
            }
            list.TryShrink();
            return false;
        }

        public sealed override void Dispose() => list.Return();
    }
}