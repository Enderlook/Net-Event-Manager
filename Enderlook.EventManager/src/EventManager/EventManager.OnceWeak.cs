using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Handles<Type> onceWeakWithArgumentHandle;
        private Handles<Type> onceWeakHandle;
        private Handles<Type2> onceWeakWithArgumentWithValueClosureHandle;
        private Handles<Type> onceWeakWithArgumentWithReferenceClosureHandle;
        private Handles<Type2> onceWeakWithValueClosureHandle;
        private Handles<Type> onceWeakWithReferenceClosureHandle;
        private Handles<Type2> onceWeakWithArgumentWithValueClosureWithHandleHandle;
        private Handles<Type> onceWeakWithArgumentWithReferenceClosureWithHandleHandle;
        private Handles<Type2> onceWeakWithValueClosureWithHandleHandle;
        private Handles<Type> onceWeakWithReferenceClosureWithHandleHandle;
        private Handles<Type> onceWeakWithArgumentWithHandleHandle;
        private Handles<Type> onceWeakWithHandleHandle;

        private Handles<Type> onceWeakWithArgumentHandleTrackResurrection;
        private Handles<Type> onceWeakHandleTrackResurrection;
        private Handles<Type2> onceWeakWithArgumentWithValueClosureHandleTrackResurrection;
        private Handles<Type> onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection;
        private Handles<Type2> onceWeakWithValueClosureHandleTrackResurrection;
        private Handles<Type> onceWeakWithReferenceClosureHandleTrackResurrection;
        private Handles<Type2> onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection;
        private Handles<Type> onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection;
        private Handles<Type2> onceWeakWithValueClosureWithHandleHandleTrackResurrection;
        private Handles<Type> onceWeakWithReferenceClosureWithHandleHandleTrackResurrection;
        private Handles<Type> onceWeakWithArgumentWithHandleHandleTrackResurrection;
        private Handles<Type> onceWeakWithHandleHandleTrackResurrection;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
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
                    ref onceWeakWithArgumentHandleTrackResurrection :
                    ref onceWeakWithArgumentHandle)
                    .GetOrCreate<MultipleWeakWithArgumentEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
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
                    ref onceWeakHandleTrackResurrection :
                    ref onceWeakHandle)
                    .GetOrCreate<MultipleWeakEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
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
                        ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection :
                        ref onceWeakWithArgumentWithValueClosureHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection :
                        ref onceWeakWithArgumentWithReferenceClosureHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, TEvent>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
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
                        ref onceWeakWithValueClosureHandleTrackResurrection :
                        ref onceWeakWithValueClosureHandle)
                        .GetOrCreate<MultipleWeakWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref onceWeakWithReferenceClosureHandleTrackResurrection :
                        ref onceWeakWithReferenceClosureHandle)
                        .GetOrCreate<MultipleWeakWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
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
                        ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithArgumentWithValueClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to execute.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
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
                        ref onceWeakWithValueClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithValueClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                            .Add(handle, closure, callback, trackResurrection);
                else
                    (trackResurrection ?
                        ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithReferenceClosureWithHandleHandle)
                        .GetOrCreate<MultipleWeakWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                            .Add(handle, closure, Unsafe.As<Action<object, object>>(callback), trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
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
                    ref onceWeakWithArgumentWithHandleHandleTrackResurrection :
                    ref onceWeakWithArgumentWithHandleHandle)
                    .GetOrCreate<MultipleWeakWithArgumentWithHandleEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <param name="trackResurrection">Whenever it should track the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
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
                    ref onceWeakWithHandleHandleTrackResurrection :
                    ref onceWeakWithHandleHandle)
                    .GetOrCreate<MultipleWeakWithHandleEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                        .Add(handle, callback, trackResurrection);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
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
                    ref onceWeakWithArgumentHandleTrackResurrection :
                    ref onceWeakWithArgumentHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithArgumentEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
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
                    ref onceWeakHandleTrackResurrection :
                    ref onceWeakHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
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
                        ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection :
                        ref onceWeakWithArgumentWithValueClosureHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection :
                        ref onceWeakWithArgumentWithReferenceClosureHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(handle, closure, Unsafe.As<Action<object, TEvent>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
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
                        ref onceWeakWithValueClosureHandleTrackResurrection :
                        ref onceWeakWithValueClosureHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref onceWeakWithReferenceClosureHandleTrackResurrection :
                        ref onceWeakWithReferenceClosureHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithClosureEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
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
                        ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithArgumentWithValueClosureWithHandleHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Closure of the callback to no longer execute.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
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
                        ref onceWeakWithValueClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithValueClosureWithHandleHandle)
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                        manager.Remove(handle, closure, callback);
                }
                else
                    if ((trackResurrection ?
                        ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection :
                        ref onceWeakWithReferenceClosureWithHandleHandle)
                        .TryGet(typeof(TEvent), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, object> manager))
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object>>(callback));
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{THandle, TEvent}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
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
                    ref onceWeakWithArgumentWithHandleHandleTrackResurrection :
                    ref onceWeakWithArgumentWithHandleHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithArgumentWithHandleEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{THandle}, bool)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <param name="trackResurrection">Whenever it was tracking the resurrection of the handle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
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
                    ref onceWeakWithHandleHandleTrackResurrection :
                    ref onceWeakWithHandleHandle)
                    .TryGet(typeof(TEvent), out MultipleWeakWithHandleEventHandle<TEvent> manager))
                    manager.Remove(handle, callback);
                globalLock.ReadEnd();
            }
        }
    }
}