using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct EventListOnce<TDelegate>
    {
        private Array<TDelegate> toRun;
        private int toRunCount;

        private Array<TDelegate> toRemove;
        private int toRemoveCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventListOnce<TDelegate> Create()
        {
            Array<TDelegate> array = Array<TDelegate>.Empty();
            return new EventListOnce<TDelegate>()
            {
                toRun = array,
                toRemove = array,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate element) => Utility.Add(ref toRun, ref toRunCount, element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate element) => Utility.Add(ref toRemove, ref toRemoveCount, element);

        public void ExtractToRun(out Array<TDelegate> toRunExtracted, out int toRunCount, out Array<TDelegate> toRemoveExtracted, out int toRemoveCount)
        {
            Utility.Extract(ref toRun, ref this.toRunCount, out toRunExtracted, out toRunCount);
            Utility.Extract(ref toRemove, ref this.toRemoveCount, out toRemoveExtracted, out toRemoveCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExtractToRunRemoved(out Array<TDelegate> toRunExtracted, out int toRunCount)
            => Utility.ExtractToRun(ref toRun, ref this.toRunCount, ref toRemove, ref toRemoveCount, out toRunExtracted, out toRunCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectToRun(Array<TDelegate> array, int count)
            => Utility.Drain(ref toRun, ref toRunCount, array, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Utility.InjectEmpty(ref toRun, ref toRunCount);
            Utility.InjectEmpty(ref toRemove, ref toRemoveCount);
        }
    }
}