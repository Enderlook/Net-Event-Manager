using System;

namespace Enderlook.EventManager
{
    internal interface IEventCollection<TDelegate> : IDisposable
    {
        void Add(TDelegate element);

        void Compact();

        void CompactAndPurge();

        List<TDelegate> GetExecutionList();

        void Remove(TDelegate element);

        void ReturnExecutionList(List<TDelegate> list);
    }
}