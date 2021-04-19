using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventListOnce<TDelegate>
    {
        private List<TDelegate> toAdd;
        private List<TDelegate> toRemove;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventListOnce<TDelegate> Create()
        {
            List<TDelegate> list = List<TDelegate>.Empty();
            return new EventListOnce<TDelegate>()
            {
                toAdd = list,
                toRemove = list,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate element) => toAdd.ConcurrentAdd(element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate element) => toRemove.ConcurrentAdd(element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<TDelegate> GetExecutionList()
        {
            Array<TDelegate> add = Array<TDelegate>.Steal(ref toAdd.Array);
            Array<TDelegate> remove = Array<TDelegate>.Steal(ref toRemove.Array);
            List<TDelegate> toAdd_ = new(add, toAdd.Count);
            List<TDelegate> toRemove_ = new(remove, toRemove.Count);
            toAdd.InjectZero();
            toRemove.InjectZero();
            toAdd_.ConcurrentRemoveFrom(ref toRemove_);
            toRemove_.Return();
            return toAdd_.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Purge()
        {
            toAdd.ConcurrentRemoveFrom(ref toRemove);
            toAdd.ExtractIfEmpty();
            toRemove.ExtractIfEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            toAdd.Return();
            toRemove.Return();
        }
    }
}