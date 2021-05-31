using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Dictionary<Type, EventHandle> onceWeakWithArgumentHandle;
        private Dictionary<Type, EventHandle> onceWeakHandle;
        private Dictionary<Type2, EventHandle> onceWeakWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> onceWeakWithValueClosureHandle;
        private Dictionary<Type, EventHandle> onceWeakWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> onceWeakWithArgumentWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithReferenceClosureWithHandleHandle;
        private Dictionary<Type2, EventHandle> onceWeakWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> onceWeakWithReferenceClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithHandleHandle;
        private Dictionary<Type, EventHandle> onceWeakWithHandleHandle;

        private Dictionary<Type, EventHandle> onceWeakWithArgumentHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> onceWeakWithArgumentWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> onceWeakWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> onceWeakWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithArgumentWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> onceWeakWithHandleHandleTrackResurrection;

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
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleWeakWithArgumentEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref onceWeakWithArgumentHandleTrackResurrection : ref onceWeakWithArgumentHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleWeakEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref onceWeakHandleTrackResurrection : ref onceWeakHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection : ref onceWeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref onceWeakWithArgumentWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, TEvent>>(callback), trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref onceWeakWithValueClosureHandleTrackResurrection : ref onceWeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref onceWeakWithReferenceClosureHandleTrackResurrection : ref onceWeakWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object>>(callback), trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback), trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref onceWeakWithValueClosureWithHandleHandleTrackResurrection : ref onceWeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection : ref onceWeakWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, object>>(callback), trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleWeakWithArgumentWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref onceWeakWithArgumentWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithHandleHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleWeakWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref onceWeakWithHandleHandleTrackResurrection : ref onceWeakWithHandleHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent}, bool)"/>.
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
                ThrowNullCallbackException();

            if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentHandleTrackResurrection : ref onceWeakWithArgumentHandle,
                typeof(TEvent), out MultipleWeakWithArgumentEventHandle<TEvent> manager))
            {
                manager.Remove(handle, callback);
                InEventEnd();
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
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref trackResurrection ? ref onceWeakHandleTrackResurrection : ref onceWeakHandle,
                typeof(TEvent), out MultipleWeakEventHandle<TEvent> manager))
            {
                manager.Remove(handle, callback);
                InEventEnd();
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
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentWithValueClosureHandleTrackResurrection : ref onceWeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref onceWeakWithArgumentWithReferenceClosureHandle,
                    typeof(TEvent), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object> manager))
                {
                    manager.Remove(handle, closure, Unsafe.As<Action<object, TEvent>>(callback));
                    InEventEnd();
                }
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
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithValueClosureHandleTrackResurrection : ref onceWeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithReferenceClosureHandleTrackResurrection : ref onceWeakWithReferenceClosureHandle,
                    typeof(TEvent), out MultipleWeakWithClosureEventHandle<TEvent, object> manager))
                {
                    manager.Remove(handle, closure, Unsafe.As<Action<object>>(callback));
                    InEventEnd();
                }
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
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithReferenceClosureWithHandleHandle,
                    typeof(TEvent), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object> manager))
                {
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback));
                    InEventEnd();
                }
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
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithValueClosureWithHandleHandleTrackResurrection : ref onceWeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref onceWeakWithReferenceClosureWithHandleHandleTrackResurrection : ref onceWeakWithReferenceClosureWithHandleHandle,
                    typeof(TEvent), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, object> manager))
                {
                    manager.Remove(handle, closure, Unsafe.As<Action<object, object>>(callback));
                    InEventEnd();
                }
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
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref trackResurrection ? ref onceWeakWithArgumentWithHandleHandleTrackResurrection : ref onceWeakWithArgumentWithHandleHandle,
                typeof(TEvent), out MultipleWeakWithArgumentWithHandleEventHandle<TEvent> manager))
            {
                manager.Remove(handle, callback);
                InEventEnd();
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
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref trackResurrection ? ref onceWeakWithHandleHandleTrackResurrection : ref onceWeakWithHandleHandle,
                typeof(TEvent), out MultipleWeakWithHandleEventHandle<TEvent> manager))
            {
                manager.Remove(handle, callback);
                InEventEnd();
            }
        }
    }
}