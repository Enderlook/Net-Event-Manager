using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        private Handles<Type> multipleStrongWithArgumentHandle;
        private Handles<Type> multipleStrongHandle;
        private Handles<Type2> multipleStrongWithArgumentWithValueClosureHandle;
        private Handles<Type> multipleStrongWithArgumentWithReferenceClosureHandle;
        private Handles<Type2> multipleStrongWithValueClosureHandle;
        private Handles<Type> multipleStrongWithReferenceClosureHandle;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Subscribe<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                multipleStrongWithArgumentHandle
                    .GetOrCreate<MultipleStrongWithArgumentEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                    .Add(callback);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                multipleStrongHandle
                    .GetOrCreate<MultipleStrongEventHandle<TEvent>, TEvent>(typeof(TEvent), this)
                    .Add(callback);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    multipleStrongWithArgumentWithValueClosureHandle
                        .GetOrCreate<MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                        .Add(callback, closure);
                else
                    multipleStrongWithArgumentWithReferenceClosureHandle
                        .GetOrCreate<MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                        .Add(Unsafe.As<Action<object, TEvent>>(callback), closure);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                    multipleStrongWithValueClosureHandle
                        .GetOrCreate<MultipleStrongWithClosureEventHandle<TEvent, TClosure>, TEvent>(new(typeof(TEvent), typeof(TClosure)), this)
                        .Add(callback, closure);
                else
                    multipleStrongWithReferenceClosureHandle
                        .GetOrCreate<MultipleStrongWithClosureEventHandle<TEvent, object>, TEvent>(typeof(TEvent), this)
                        .Add(Unsafe.As<Action<object>>(callback), closure);
                globalLock.ReadEnd();
            }
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (multipleStrongWithArgumentHandle
                    .TryGet(typeof(TEvent), out MultipleStrongWithArgumentEventHandle<TEvent> manager))
                    manager.Remove(callback);
                globalLock.ReadEnd();
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (multipleStrongHandle
                    .TryGet(typeof(TEvent), out MultipleStrongEventHandle<TEvent> manager))
                    manager.Remove(callback);
                globalLock.ReadEnd();
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if (multipleStrongWithArgumentWithValueClosureHandle
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(callback, closure);
                }
                else
                {
                    if (multipleStrongWithArgumentWithReferenceClosureHandle
                        .TryGet(typeof(TEvent), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(Unsafe.As<Action<object, TEvent>>(callback), closure);
                }
                globalLock.ReadEnd();
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
                ThrowNullCallback();

            globalLock.ReadBegin();
            if (isDisposedOrDisposing)
                ThrowObjectDisposedExceptionAndEndGlobalRead();
            else
            {
                if (typeof(TClosure).IsValueType)
                {
                    if (multipleStrongWithValueClosureHandle
                        .TryGet(new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithClosureEventHandle<TEvent, TClosure> manager))
                        manager.Remove(callback, closure);
                }
                else
                {
                    if (multipleStrongWithReferenceClosureHandle
                        .TryGet(typeof(TEvent), out MultipleStrongWithClosureEventHandle<TEvent, object> manager))
                        manager.Remove(Unsafe.As<Action<object>>(callback), closure);
                }
                globalLock.ReadEnd();
            }
        }
    }
}