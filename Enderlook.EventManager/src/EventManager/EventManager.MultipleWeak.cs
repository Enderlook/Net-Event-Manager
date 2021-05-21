using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Handles<Type> multipleWeakWithArgumentHandle;
        private Handles<Type> multipleWeakHandle;
        private Handles<Type2> multipleWeakWithArgumentWithValueClosureHandle;
        private Handles<Type> multipleWeakWithArgumentWithReferenceClosureHandle;
        private Handles<Type2> multipleWeakWithValueClosureHandle;
        private Handles<Type> multipleWeakWithReferenceClosureHandle;
        private Handles<Type2> multipleWeakWithArgumentWithValueClosureWithHandleHandle;
        private Handles<Type> multipleWeakWithArgumentWithReferenceClosureWithHandleHandle;
        private Handles<Type2> multipleWeakWithValueClosureWithHandleHandle;
        private Handles<Type> multipleWeakWithReferenceClosureWithHandleHandle;
        private Handles<Type> multipleWeakWithArgumentWithHandleHandle;
        private Handles<Type> multipleWeakWithHandleHandle;

        private Handles<Type> multipleWeakWithArgumentHandleTrackResurrection;
        private Handles<Type> multipleWeakHandleTrackResurrection;
        private Handles<Type2> multipleWeakWithArgumentWithValueClosureHandleTrackResurrection;
        private Handles<Type> multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection;
        private Handles<Type2> multipleWeakWithValueClosureHandleTrackResurrection;
        private Handles<Type> multipleWeakWithReferenceClosureHandleTrackResurrection;
        private Handles<Type2> multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection;
        private Handles<Type> multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection;
        private Handles<Type2> multipleWeakWithValueClosureWithHandleHandleTrackResurrection;
        private Handles<Type> multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection;
        private Handles<Type> multipleWeakWithArgumentWithHandleHandleTrackResurrection;
        private Handles<Type> multipleWeakWithHandleHandleTrackResurrection;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                (trackResurrection ?
                    ref multipleWeakWithArgumentHandleTrackResurrection :
                    ref multipleWeakWithArgumentHandle)
                    .GetOrCreate<MultipleWeakWithArgumentEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                (trackResurrection ?
                    ref multipleWeakHandleTrackResurrection :
                    ref multipleWeakHandle)
                    .GetOrCreate<MultipleWeakEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    (trackResurrection ?
                        ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithValueClosureHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithReferenceClosureHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, TEvent>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    (trackResurrection ?
                        ref multipleWeakWithValueClosureHandleTrackResurrection :
                        ref multipleWeakWithValueClosureHandle)
                        .GetOrCreate<MultipleWeakWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref multipleWeakWithReferenceClosureHandleTrackResurrection :
                        ref multipleWeakWithReferenceClosureHandle)
                        .GetOrCreate<MultipleWeakWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    (trackResurrection ?
                        ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithValueClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    (trackResurrection ?
                        ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithValueClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithReferenceClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, object>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                (trackResurrection ?
                    ref multipleWeakWithArgumentWithHandleHandleTrackResurrection :
                    ref multipleWeakWithArgumentWithHandleHandle)
                    .GetOrCreate<MultipleWeakWithArgumentWithHandleEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                (trackResurrection ?
                    ref multipleWeakWithHandleHandleTrackResurrection :
                    ref multipleWeakWithHandleHandle)
                    .GetOrCreate<MultipleWeakWithHandleEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if ((trackResurrection ?
                    ref multipleWeakWithArgumentHandleTrackResurrection :
                    ref multipleWeakWithArgumentHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithArgumentEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if ((trackResurrection ?
                    ref multipleWeakHandleTrackResurrection :
                    ref multipleWeakHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if ((trackResurrection ?
                        ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithValueClosureHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithReferenceClosureHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(handle, closure, Unsafe.As<Action<object, TEvent>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if ((trackResurrection ?
                        ref multipleWeakWithValueClosureHandleTrackResurrection :
                        ref multipleWeakWithValueClosureHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref multipleWeakWithReferenceClosureHandleTrackResurrection :
                        ref multipleWeakWithReferenceClosureHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithClosureEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if ((trackResurrection ?
                        ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithValueClosureWithHandleHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if ((trackResurrection ?
                        ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithValueClosureWithHandleHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref multipleWeakWithReferenceClosureWithHandleHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if ((trackResurrection ?
                    ref multipleWeakWithArgumentWithHandleHandleTrackResurrection :
                    ref multipleWeakWithArgumentWithHandleHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithHandleEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if ((trackResurrection ?
                    ref multipleWeakWithHandleHandleTrackResurrection :
                    ref multipleWeakWithHandleHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithHandleEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }
    }
}