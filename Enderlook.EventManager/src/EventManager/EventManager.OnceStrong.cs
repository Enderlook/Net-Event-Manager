using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Handles<Type> onceStrongWithArgumentHandle;
        private Handles<Type> onceStrongHandle;

        private Handles<Type2> onceStrongWithArgumentWithValueClosureHandle;
        private Handles<Type> onceStrongWithArgumentWithReferenceClosureHandle;
        private Handles<Type2> onceStrongWithValueClosureHandle;
        private Handles<Type> onceStrongWithReferenceClosureHandle;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute the next time the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void SubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                onceStrongWithArgumentHandle
                    .GetOrCreate<OnceStrongWithArgumentEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                    .Add(callback);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                onceStrongHandle
                    .GetOrCreate<OnceStrongEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                    .Add(callback);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    onceStrongWithArgumentWithValueClosureHandle
                        .GetOrCreate<OnceStrongWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                        .Add(callback, closure);
                else
                    onceStrongWithArgumentWithReferenceClosureHandle
                        .GetOrCreate<OnceStrongWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                        .Add(Unsafe.As<Action<object, TEvent>>(callback), closure);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    onceStrongWithValueClosureHandle
                        .GetOrCreate<OnceStrongWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                        .Add(callback, closure);
                else
                    onceStrongWithReferenceClosureHandle
                        .GetOrCreate<OnceStrongWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                        .Add(Unsafe.As<Action<object>>(callback), closure);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void UnsubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (onceStrongWithArgumentHandle
                    .TryGet(typeof(TEvent), out OnceStrongWithArgumentEventHandle<TEvent> manager))
                    manager.Remove(callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void UnsubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (onceStrongHandle
                    .TryGet(typeof(TEvent), out OnceStrongEventHandle<TEvent> manager))
                    manager.Remove(callback);
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent, TClosure}(TClosure, Action{TClosure, TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void UnsubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure, TEvent> callback)
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
                    if (onceStrongWithArgumentWithValueClosureHandle
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out OnceStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(callback, closure);
                }
                else
                {
                    if (onceStrongWithArgumentWithReferenceClosureHandle
                        .TryGet(typeof(TEvent), out OnceStrongWithArgumentWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(Unsafe.As<Action<object, TEvent>>(callback), closure);
                }
                globalLock.ReadEnd();
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent, TClosure}(TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void UnsubscribeOnce<TEvent, TClosure>(TClosure closure, Action<TClosure> callback)
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
                    if (onceStrongWithValueClosureHandle
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out OnceStrongWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(callback, closure);
                }
                else
                {
                    if (onceStrongWithReferenceClosureHandle
                        .TryGet(typeof(TEvent), out OnceStrongWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(Unsafe.As<Action<object>>(callback), closure);
                }
                globalLock.ReadEnd();
            }
        }
    }
}