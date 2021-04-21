using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IEventCollection<TDelegate>
    {
        private const int LOCKED = 2;
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

            int value;
            do
            {
                value = Interlocked.Exchange(ref toExecuteState, LOCKED);
            } while (value == LOCKED);

            if (value == BORROWED)
                toExecute = toExecute.ConcurrentClone();

            Compact();

            List<TDelegate> result = toExecute;
            if (result.Count == FREE)
            {
                toExecuteState = value;
                return List<TDelegate>.Empty();
            }
            toExecuteState = BORROWED;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReturnExecutionList(List<TDelegate> list)
        {
            int value;
            do
            {
                value = Interlocked.Exchange(ref toExecuteState, LOCKED);
            } while (value == LOCKED);

            if (list.Array.AsObject == toExecute.Array.AsObject)
            {
                toExecuteState = FREE;
                return;
            }

            toExecuteState = value;
            list.Return();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CompactAndPurge()
        {
            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            toExecute_.AddFrom(ref toAdd_);
            List<TDelegate>.Overwrite(ref toAdd, List<TDelegate>.Empty());
            toAdd_.Return();

            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toExecute.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, List<TDelegate>.Empty());
            toRemove_.Return();

            if (toExecute_.Count == 0)
            {
                List<TDelegate>.Overwrite(ref toExecute, List<TDelegate>.Empty());
                toExecute_.Return();
            }
            else
                List<TDelegate>.Overwrite(ref toExecute, toExecute_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compact()
        {
            List<TDelegate> toExecute_ = List<TDelegate>.Steal(ref toExecute);
            List<TDelegate> toAdd_ = List<TDelegate>.Steal(ref toAdd);
            toExecute_.AddFrom(ref toAdd_);
            List<TDelegate>.Overwrite(ref toAdd, toAdd_);

            List<TDelegate> toRemove_ = List<TDelegate>.Steal(ref toRemove);
            toExecute.RemoveFrom(ref toRemove);
            List<TDelegate>.Overwrite(ref toRemove, toRemove_);

            List<TDelegate>.Overwrite(ref toExecute, toExecute_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            toExecute.Return();
            toAdd.Return();
            toRemove.Return();
        }
    }
}