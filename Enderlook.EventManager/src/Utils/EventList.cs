using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IEventCollection<TDelegate>
    {
        private const int BORROWED = 1;
        private const int FREE = 0;

        private List<TDelegate> toExecute;
        private int toExecuteState;
        private List<TDelegate> toAdd;
        private List<TDelegate> toRemove;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventList<TDelegate> Create()
        {
            List<TDelegate> list = List<TDelegate>.Empty();
            return new EventList<TDelegate>()
            {
                toExecute = list,
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
            // We try to avoid the cloning of the list by borrowing it on the first execution

            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);

            if (toExecuteState == BORROWED)
                toExecute_ = toExecute.Clone();

            Compact(toExecute_);

            if (toExecute_.Count == 0)
            {
                toExecuteState = FREE;
                List<TDelegate>.Overwrite(ref toExecute, toExecute_);
                return List<TDelegate>.Empty();
            }
            else
            {
                toExecuteState = BORROWED;
                List<TDelegate>.Overwrite(ref toExecute, toExecute_);
                return toExecute_;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReturnExecutionList(List<TDelegate> list)
        {
            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);

            if (list.Array.AsObject == toExecute_.Array.AsObject)
            {
                List<TDelegate>.Overwrite(ref toExecute, toExecute_);
                toExecuteState = FREE;
                return;
            }

            List<TDelegate>.Overwrite(ref toExecute, toExecute_);
            list.Return();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CompactAndPurge()
        {
            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);

            if (toExecuteState == BORROWED)
                toExecute_ = toExecute.Clone();
            toExecuteState = FREE;

            toExecute.RemoveWeakHandlesIfHas();
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            toExecute_.AddFrom(ref toAdd_);
            List<TDelegate>.Overwrite(ref toAdd, List<TDelegate>.Empty());
            toAdd_.Return();

            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toExecute.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, List<TDelegate>.Empty());
            toRemove_.Return();

            if (toExecute_.Count == 0)
                RarePath(ref toExecute, toExecute_);
            else
                List<TDelegate>.Overwrite(ref toExecute, toExecute_);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void RarePath(ref List<TDelegate> toExecute, List<TDelegate> toExecute_)
            {
                List<TDelegate>.Overwrite(ref toExecute, List<TDelegate>.Empty());
                toExecute_.Return();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compact()
        {
            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);

            if (toExecuteState == BORROWED)
                toExecute_ = toExecute.Clone();
            toExecuteState = FREE;

            Compact(toExecute_);
            List<TDelegate>.Overwrite(ref toExecute, toExecute_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Compact(List<TDelegate> toExecute)
        {
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            toExecute.AddFrom(ref toAdd_);
            List<TDelegate>.Overwrite(ref toAdd, toAdd_);

            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toExecute.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, toRemove_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            toExecute.Dispose();
            toAdd.Dispose();
            toRemove.Dispose();
        }
    }
}