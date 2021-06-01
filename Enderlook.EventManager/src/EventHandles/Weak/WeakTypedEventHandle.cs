using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal abstract class WeakTypedEventHandle<TEvent, TElement> : TypedEventHandle<TEvent>
        where TElement : IEquatable<TElement>
    {
        protected ValueList<WeakDelegate<TElement>> list = ValueList<WeakDelegate<TElement>>.Create();

        public sealed override bool IsEmpty {
            get {
                Debug.Assert(!list.IsLocked);
                return list.Count > 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Add(GCHandle handle, TElement callback) => list.ConcurrentAdd(new WeakDelegate<TElement>(handle, callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Remove<THandle>(THandle handle, TElement callback)
            where THandle : class
            => WeakDelegate<TElement>.ConcurrentRemove(ref list, handle, callback);

        public sealed override bool Purge()
        {
            Debug.Assert(!list.IsLocked);

            if (list.Count == 0)
                goto empty;

            WeakDelegate<TElement>.FreeExpired(ref list);
            if (list.Count == 0)
                goto empty;

            list.TryShrink();
            return false;

            empty:
            list.Return();
            list = ValueList<WeakDelegate<TElement>>.Create();
            return true;
        }

        public sealed override void Dispose()
        {
            int count = list.Count;
            WeakDelegate<TElement>[] array = list.ArrayUnlocked;

            if (unchecked((uint)count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < count; i++)
                array[i].Free();

            list.Return();
        }
    }
}