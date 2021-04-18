using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventList<TDelegate> : IDisposable
    {
        private List<TDelegate> toExecute;
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
            Purge();
            return toExecute.Clone();
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