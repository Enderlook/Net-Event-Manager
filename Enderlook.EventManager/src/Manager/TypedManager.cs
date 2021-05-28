using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class TypedManager<TEvent> : Manager
    {
        private ValueList<TypedEventHandle<TEvent>> managers = ValueList<TypedEventHandle<TEvent>>.Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<TManager>(TManager manager)
            where TManager : TypedEventHandle<TEvent>, new()
            => managers.ConcurrentAdd(manager);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(EventManager manager, TEvent argument)
        {
            ValueList<SliceWithEventHandle> stored = ValueList<SliceWithEventHandle>.Create(managers.ConcurrentCount());
            {
                int index = 0;
                while (managers.ConcurrentTryMoveNext(ref index, out TypedEventHandle<TEvent> eventHandle))
                    stored.Add(eventHandle.ConcurrentGetRaiser());
                manager.InEventEnd();

                for (int i = 0; i < stored.Count; i++)
                {
                    SliceWithEventHandle handle = stored.Get(i);
                    CastUtils.ExpectAssignableType<TypedEventHandle<TEvent>>(handle.handle).ConcurrentRaise(handle.slice, argument);
                }
            }
            stored.Return();
        }

        public override bool Purge()
        {
            ValueList<TypedEventHandle<TEvent>> managers_ = managers;
            Debug.Assert(!managers_.IsLocked);

            int j = 0;
            for (int i = 0; i < managers_.Count; i++)
            {
                TypedEventHandle<TEvent> element = managers_.Get(i);
                if (!element.IsEmpty)
                    managers.Get(j++) = element;
            }

            if (managers_.Count == 0)
            {
                managers_.Return();
                managers = ValueList<TypedEventHandle<TEvent>>.Create();
                return true;
            }

            managers_.TryShrink();
            managers = managers_;
            return false;
        }

        public override void Dispose() => managers.Return();
    }
}