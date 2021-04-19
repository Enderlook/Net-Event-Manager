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
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toAdd_.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, toRemove_);
            List<TDelegate>.Overwrite(ref toAdd, List<TDelegate>.Empty());
            return toAdd_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Purge()
        {
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toAdd_.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, List<TDelegate>.Empty());
            toRemove_.Return();
            if (toAdd_.Count == 0)
            {
                List<TDelegate>.Overwrite(ref toAdd, List<TDelegate>.Empty());
                toAdd_.Return();
            }
            else
                List<TDelegate>.Overwrite(ref toAdd, toAdd_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            toAdd.Return();
            toRemove.Return();
        }
    }
}