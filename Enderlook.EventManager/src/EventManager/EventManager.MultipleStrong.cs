using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Dictionary<Type, EventHandle> multipleStrongWithArgumentHandle;
        private Dictionary<Type, EventHandle> multipleStrongHandle;
        private Dictionary<Type2, EventHandle> multipleStrongWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle> multipleStrongWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> multipleStrongWithValueClosureHandle;
        private Dictionary<Type, EventHandle> multipleStrongWithReferenceClosureHandle;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Subscribe<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongWithArgumentEventHandle<TEvent>, TEvent>(
                ref multipleStrongWithArgumentHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Subscribe<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongEventHandle<TEvent>, TEvent>(ref multipleStrongHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Subscribe<TEvent, TClosure>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref multipleStrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(
                    ref multipleStrongWithArgumentWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object, TEvent>>(callback), closure);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Subscribe<TEvent, TClosure>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref multipleStrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithClosureEventHandle<TEvent, object>, TEvent>(
                    ref multipleStrongWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object>>(callback), closure);
            InEventEnd();
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref multipleStrongWithArgumentHandle, typeof(TEvent), out MultipleStrongWithArgumentEventHandle<TEvent> manager))
            {
                manager.Remove(callback);
                InEventEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref multipleStrongHandle, typeof(TEvent), out MultipleStrongEventHandle<TEvent> manager))
            {
                manager.Remove(callback);
                InEventEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent, TClosure}(TClosure, Action{TClosure, TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe<TEvent, TClosure>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref multipleStrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(callback, closure);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref multipleStrongWithArgumentWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object> manager))
                {
                    manager.Remove(Unsafe.As<Action<object, TEvent>>(callback), closure);
                    InEventEnd();
                }
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent, TClosure}(TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe<TEvent, TClosure>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref multipleStrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(callback, closure);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref multipleStrongWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithClosureEventHandle<TEvent, object> manager))
                {
                    manager.Remove(Unsafe.As<Action<object>>(callback), closure);
                    InEventEnd();
                }
            }
        }
    }
}