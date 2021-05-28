using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentHandle;
        private Dictionary<Type, EventHandle> multipleWeakHandle;
        private Dictionary<Type2, EventHandle> multipleWeakWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> multipleWeakWithValueClosureHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> multipleWeakWithArgumentWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithReferenceClosureWithHandleHandle;
        private Dictionary<Type2, EventHandle> multipleWeakWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithReferenceClosureWithHandleHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithHandleHandle;
        private Dictionary<Type, EventHandle> multipleWeakWithHandleHandle;

        private Dictionary<Type, EventHandle> multipleWeakWithArgumentHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> multipleWeakWithArgumentWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> multipleWeakWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type2, EventHandle> multipleWeakWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithArgumentWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle> multipleWeakWithHandleHandleTrackResurrection;

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

            GetOrCreate<Type, MultipleWeakWithArgumentEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithArgumentHandleTrackResurrection : ref multipleWeakWithArgumentHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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

            GetOrCreate<Type, MultipleWeakEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref multipleWeakHandleTrackResurrection : ref multipleWeakHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection : ref multipleWeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref multipleWeakWithArgumentWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, TEvent>>(callback), trackResurrection);
            InEventEnd();
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

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref multipleWeakWithValueClosureHandleTrackResurrection : ref multipleWeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithReferenceClosureHandleTrackResurrection : ref multipleWeakWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object>>(callback), trackResurrection);
            InEventEnd();
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

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, object, TEvent>>(callback), trackResurrection);
            InEventEnd();
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

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure>, TEvent>(
                    ref trackResurrection ? ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection : ref multipleWeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureWithHandleEventHandle<TEvent, object>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection : ref multipleWeakWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, closure, Unsafe.As<Action<object, object>>(callback), trackResurrection);
            InEventEnd();
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

            GetOrCreate<Type, MultipleWeakWithArgumentWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithArgumentWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithHandleHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
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

            GetOrCreate<Type, MultipleWeakWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref multipleWeakWithHandleHandleTrackResurrection : ref multipleWeakWithHandleHandle,
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
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentHandleTrackResurrection : ref multipleWeakWithArgumentHandle,
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
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGet(ref trackResurrection ? ref multipleWeakHandleTrackResurrection : ref multipleWeakHandle,
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
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentWithValueClosureHandleTrackResurrection : ref multipleWeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref multipleWeakWithArgumentWithReferenceClosureHandle,
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
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithValueClosureHandleTrackResurrection : ref multipleWeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithReferenceClosureHandleTrackResurrection : ref multipleWeakWithReferenceClosureHandle,
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
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithReferenceClosureWithHandleHandle,
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
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithValueClosureWithHandleHandleTrackResurrection : ref multipleWeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref trackResurrection ? ref multipleWeakWithReferenceClosureWithHandleHandleTrackResurrection : ref multipleWeakWithReferenceClosureWithHandleHandle,
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
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGet(ref trackResurrection ? ref multipleWeakWithArgumentWithHandleHandleTrackResurrection : ref multipleWeakWithArgumentWithHandleHandle,
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
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGet(ref trackResurrection ? ref multipleWeakWithHandleHandleTrackResurrection : ref multipleWeakWithHandleHandle,
                typeof(TEvent), out MultipleWeakWithHandleEventHandle<TEvent> manager))
            {
                manager.Remove(handle, callback);
                InEventEnd();
            }
        }
    }
}