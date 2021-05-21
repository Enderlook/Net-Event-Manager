using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    public sealed partial class EventManager : IDisposable
    {
        private ReadWriteLock globalLock;
        private bool isDisposedOrDisposing;

        private ReadWriteLock managersDictionaryLock;
        private Dictionary<Type, Manager> managersDictionary;
        private ValueList<Manager> managersList;

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event</typeparam>
        /// <param name="argument">Arguments of this event.</param>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Raise<TEvent>(TEvent argument)
        {
            managersDictionaryLock.ReadBegin();
            bool found = managersDictionary.TryGetValue(typeof(TEvent), out Manager managers);
            managersDictionaryLock.ReadEnd();
            if (found)
                CastUtils.ExpectExactType<TypedManager<TEvent>>(managers).Raise(argument);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (isDisposedOrDisposing)
                return;

            globalLock.WriteBegin();
            {
                if (isDisposedOrDisposing)
                {
                    globalLock.WriteEnd();
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
            globalLock.WriteEnd();
        }

        ~EventManager() => Dispose();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowObjectDisposedExceptionAndEndGlobalRead()
        {
            globalLock.ReadEnd();
            throw new ObjectDisposedException("Event Manager");
        }
    }
}