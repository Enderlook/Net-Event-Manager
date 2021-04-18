using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IDisposable
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
                toExecute = toExecute.Clone();

            Purge();

            List<TDelegate> result = toExecute;
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
        public void Purge()
        {
            toExecute.ConcurrentAddFrom(ref toAdd);
            toExecute.ConcurrentRemoveFrom(ref toRemove);
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