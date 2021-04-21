using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IEventCollection<TDelegate>
    {
        private List<TDelegate> toExecute;
        private int toExecuteIsBeingUsed;
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
                value = Interlocked.Exchange(ref toExecuteIsBeingUsed, 2);
            } while (value == 2);

            if (value == 1)
                toExecute = toExecute.ConcurrentClone();

            Compact();

            List<TDelegate> result = toExecute;
            if (result.Count == 0)
            {
                toExecuteIsBeingUsed = value;
                return List<TDelegate>.Empty();
            }
            toExecuteIsBeingUsed = 1;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReturnExecutionList(List<TDelegate> list)
        {
            int value;
            do
            {
                value = Interlocked.Exchange(ref toExecuteIsBeingUsed, 2);
            } while (value == 2);

            if (list.Array.AsObject == toExecute.Array.AsObject)
            {
                toExecuteIsBeingUsed = 0;
                return;
            }

            toExecuteIsBeingUsed = value;
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