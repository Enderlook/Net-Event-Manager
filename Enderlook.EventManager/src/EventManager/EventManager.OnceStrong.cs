using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Dictionary<Type, EventHandle> onceStrongWithArgumentHandle;
        private Dictionary<Type, EventHandle> onceStrongHandle;
        private Dictionary<Type2, EventHandle> onceStrongWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle> onceStrongWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle> onceStrongWithValueClosureHandle;
        private Dictionary<Type, EventHandle> onceStrongWithReferenceClosureHandle;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void SubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongWithArgumentEventHandle<TEvent>, TEvent>(
                ref onceStrongWithArgumentHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void SubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongEventHandle<TEvent>, TEvent>(ref onceStrongHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void SubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref onceStrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(
                    ref onceStrongWithArgumentWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object, TEvent>>(callback), closure);
            InEventEnd();
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void SubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithClosureEventHandle<TEvent, TClosure>, TEvent>(
                    ref onceStrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithClosureEventHandle<TEvent, object>, TEvent>(
                    ref onceStrongWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object>>(callback), closure);
            InEventEnd();
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void UnsubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref onceStrongWithArgumentHandle, typeof(TEvent), out MultipleStrongWithArgumentEventHandle<TEvent> manager))
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
        public void UnsubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref onceStrongHandle, typeof(TEvent), out MultipleStrongEventHandle<TEvent> manager))
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
        public void UnsubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref onceStrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(callback, closure);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref onceStrongWithArgumentWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object> manager))
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
        public void UnsubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGet(ref onceStrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithClosureEventHandle<TEvent, TClosure> manager))
                {
                    manager.Remove(callback, closure);
                    InEventEnd();
                }
            }
            else
            {
                if (TryGet(ref onceStrongWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithClosureEventHandle<TEvent, object> manager))
                {
                    manager.Remove(Unsafe.As<Action<object>>(callback), closure);
                    InEventEnd();
                }
            }
        }
    }
}