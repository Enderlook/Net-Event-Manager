using System;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager : IDisposable
    {
        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (isDisposedOrDisposing)
                return;

            MassiveWriteBegin();
            {
                if (isDisposedOrDisposing)
                {
                    WriteEnd();
                    return;
                }

                GC.SuppressFinalize(this);

                isDisposedOrDisposing = true;
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
                for (int i = 0; i < managers.Count; i++)
                    managers.Get(i).Dispose();
            }
            WriteEnd();
        }
    }
}