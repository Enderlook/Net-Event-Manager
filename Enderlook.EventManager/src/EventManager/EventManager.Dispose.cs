using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager : IDisposable
    {
        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (state == IS_DISPOSED_OR_DISPOSING)
                return;

            MassiveWriteBegin();
            {
                if (state == IS_DISPOSED_OR_DISPOSING)
                {
                    WriteEnd();
                    return;
                }

                GC.SuppressFinalize(this);

                while (Interlocked.Exchange(ref stateLock, 1) != 0) ; ;
                {
                    state = IS_DISPOSED_OR_DISPOSING;
                }
                stateLock = 0;

                managersDictionary = default;

                multipleStrongWithArgumentHandle = default;
                multipleStrongHandle = default;
                multipleStrongWithArgumentWithValueClosureHandle = default;
                multipleStrongWithArgumentWithReferenceClosureHandle = default;
                multipleStrongWithValueClosureHandle = default;
                multipleStrongWithReferenceClosureHandle = default;

                onceStrongWithArgumentHandle = default;
                onceStrongHandle = default;
                onceStrongWithArgumentWithValueClosureHandle = default;
                onceStrongWithArgumentWithReferenceClosureHandle = default;
                onceStrongWithValueClosureHandle = default;
                onceStrongWithReferenceClosureHandle = default;

                multipleWeakWithArgumentHandle = default;
                multipleWeakHandle = default;
                multipleWeakWithArgumentWithValueClosureHandle = default;
                multipleWeakWithArgumentWithReferenceClosureHandle = default;
                multipleWeakWithValueClosureHandle = default;
                multipleWeakWithReferenceClosureHandle = default;
                multipleWeakWithArgumentWithValueClosureWithHandleHandle = default;
                multipleWeakWithArgumentWithReferenceClosureWithHandleHandle = default;
                multipleWeakWithValueClosureWithHandleHandle = default;
                multipleWeakWithReferenceClosureWithHandleHandle = default;
                multipleWeakWithArgumentWithHandleHandle = default;
                multipleWeakWithHandleHandle = default;
                multipleWeakWithArgumentHandleTrackResurrection = default;
                multipleWeakHandleTrackResurrection = default;
                multipleWeakWithArgumentWithValueClosureHandleTrackResurrection = default;
                multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection = default;
                multipleWeakWithValueClosureHandleTrackResurrection = default;
                multipleWeakWithReferenceClosureHandleTrackResurrection = default;
                multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection = default;
                multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection = default;
                multipleWeakWithValueClosureWithHandleHandleTrackResurrection = default;
                multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection = default;
                multipleWeakWithArgumentWithHandleHandleTrackResurrection = default;
                multipleWeakWithHandleHandleTrackResurrection = default;

                onceWeakWithArgumentHandle = default;
                onceWeakHandle = default;
                onceWeakWithArgumentWithValueClosureHandle = default;
                onceWeakWithArgumentWithReferenceClosureHandle = default;
                onceWeakWithValueClosureHandle = default;
                onceWeakWithReferenceClosureHandle = default;
                onceWeakWithArgumentWithValueClosureWithHandleHandle = default;
                onceWeakWithArgumentWithReferenceClosureWithHandleHandle = default;
                onceWeakWithValueClosureWithHandleHandle = default;
                onceWeakWithReferenceClosureWithHandleHandle = default;
                onceWeakWithArgumentWithHandleHandle = default;
                onceWeakWithHandleHandle = default;
                onceWeakWithArgumentHandleTrackResurrection = default;
                onceWeakHandleTrackResurrection = default;
                onceWeakWithArgumentWithValueClosureHandleTrackResurrection = default;
                onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection = default;
                onceWeakWithValueClosureHandleTrackResurrection = default;
                onceWeakWithReferenceClosureHandleTrackResurrection = default;
                onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection = default;
                onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection = default;
                onceWeakWithValueClosureWithHandleHandleTrackResurrection = default;
                onceWeakWithReferenceClosureWithHandleHandleTrackResurrection = default;
                onceWeakWithArgumentWithHandleHandleTrackResurrection = default;
                onceWeakWithHandleHandleTrackResurrection = default;

                ValueList<Manager> managers = managersList;
                managersList = default;
                Parallel.For(0, managers.Count, (i) => managers.Get(i).Dispose());
            }
            WriteEnd();
        }
    }
}