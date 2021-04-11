using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    /// <typeparam name="TEventBase">Base type of all events. Useful to determine a common ground.</typeparam>
    public sealed class EventManager<TEventBase> : IDisposable
    {
        private ReadWriterLock simpleCallbacksLocker;
        // `object` is actually `SimpleHandle<TEvent>`.
        private readonly Dictionary<Type, object> simpleCallbacks = new Dictionary<Type, object>();

        private ReadWriterLock closureCallbacksLocker;
        // `object` is actually `HeapClosureHandle<TClosure, TEvent>`.
        private readonly Dictionary<(Type, Type), object> closureCallbacks = new Dictionary<(Type, Type), object>();

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Subscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateSimpleHandle<TEvent>().Subscribe(callback);
        }

        /// <inheritdoc cref="Subscribe{TEvent}(Action{TEvent})"/>
        public void Subscribe<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateSimpleHandle<TEvent>().Subscribe(callback);
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void SubscribeOnce<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateSimpleHandle<TEvent>().SubscribeOnce(callback);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateSimpleHandle<TEvent>().SubscribeOnce(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                handle.Unsubscribe(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                handle.Unsubscribe(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                handle.UnsubscribeOnce(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                handle.UnsubscribeOnce(callback);
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                GetOrCreateClosureHandle<TClosure, TEvent>().handle.Subscribe(callback, closure);
            else
                GetOrCreateSimpleHandle<TEvent>().Subscribe(callback, closure);
        }

        /// <inheritdoc cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                GetOrCreateClosureHandle<TClosure, TEvent>().handle.Subscribe(callback, closure);
            else
                GetOrCreateSimpleHandle<TEvent>().Subscribe(callback, closure);
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                GetOrCreateClosureHandle<TClosure, TEvent>().handle.Subscribe(callback, closure);
            else
                GetOrCreateSimpleHandle<TEvent>().Subscribe(callback, closure);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                GetOrCreateClosureHandle<TClosure, TEvent>().handle.Subscribe(callback, closure);
            else
                GetOrCreateSimpleHandle<TEvent>().Subscribe(callback, closure);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
            }
            else
            {
                if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
            }
            else
            {
                if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
            }
            else
            {
                if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
            }
            else
            {
                if (TryGetTypeHandle(out SimpleHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument) where TEvent : TEventBase
        {
            if (TryGetTypeHandle(out SimpleHandle<TEvent> simpleHandler))
                simpleHandler.Raise(eventArgument);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            simpleCallbacksLocker.WriteBegin();
            try
            {
                closureCallbacksLocker.WriteBegin();
                try
                {
                    foreach (SimpleHandle handle in simpleCallbacks.Values)
                        handle.Dispose();
                    simpleCallbacks.Clear();
                    closureCallbacks.Clear();
                }
                finally
                {
                    closureCallbacksLocker.WriteEnd();
                }
            }
            finally
            {
                simpleCallbacksLocker.WriteEnd();
            }
        }

        /// <summary>
        /// Force the purge of removed delegates.
        /// </summary>
        public void Purge()
        {
            simpleCallbacksLocker.WriteBegin();
            try
            {
                closureCallbacksLocker.WriteBegin();
                try
                {
                    foreach (SimpleHandle handle in simpleCallbacks.Values)
                        handle.Purge();
                }
                finally
                {
                    closureCallbacksLocker.WriteEnd();
                }
            }
            finally
            {
                simpleCallbacksLocker.WriteEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SimpleHandle<TEvent> GetOrCreateSimpleHandle<TEvent>()
        {
            Type key = typeof(TEvent);

            simpleCallbacksLocker.ReadBegin();
            {
                if (simpleCallbacks.TryGetValue(key, out object obj))
                {
                    simpleCallbacksLocker.ReadEnd();
                    Debug.Assert(obj is SimpleHandle<TEvent>);
                    return Unsafe.As<SimpleHandle<TEvent>>(obj);
                }
            }
            simpleCallbacksLocker.ReadEnd();

            return CreateSimpleHandle(key);

            [MethodImpl(MethodImplOptions.NoInlining)]
            SimpleHandle<TEvent> CreateSimpleHandle(Type key)
            {
                simpleCallbacksLocker.WriteBegin();
                {
                    if (simpleCallbacks.TryGetValue(key, out object obj))
                    {
                        simpleCallbacksLocker.WriteEnd();
                        Debug.Assert(obj is SimpleHandle<TEvent>);
                        return Unsafe.As<SimpleHandle<TEvent>>(obj);
                    }
                    SimpleHandle<TEvent> handle = new SimpleHandle<TEvent>();
                    simpleCallbacks[key] = handle;
                    simpleCallbacksLocker.WriteEnd();
                    return handle;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetTypeHandle<TEvent>(out SimpleHandle<TEvent> handle)
        {
            Type key = typeof(TEvent);
            simpleCallbacksLocker.ReadBegin();
            if (!simpleCallbacks.TryGetValue(key, out object obj))
            {
                simpleCallbacksLocker.ReadEnd();
                handle = null;
                return false;
            }
            Debug.Assert(obj is SimpleHandle<TEvent>);
            handle = Unsafe.As<SimpleHandle<TEvent>>(obj);
            simpleCallbacksLocker.ReadEnd();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HeapClosureHandle<TClosure, TEvent> GetOrCreateClosureHandle<TClosure, TEvent>()
        {
            (Type, Type) key = (typeof(TClosure), typeof(TEvent));

            closureCallbacksLocker.ReadBegin();
            if (closureCallbacks.TryGetValue(key, out object obj))
            {
                closureCallbacksLocker.ReadEnd();
                Debug.Assert(obj is HeapClosureHandle<TClosure, TEvent>);
                return Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(obj);
            }
            closureCallbacksLocker.ReadEnd();

            return CreateClosureHandle(key);

            [MethodImpl(MethodImplOptions.NoInlining)]
            HeapClosureHandle<TClosure, TEvent> CreateClosureHandle((Type, Type) key)
            {
                closureCallbacksLocker.WriteBegin();
                if (closureCallbacks.TryGetValue(key, out object obj))
                {
                    closureCallbacksLocker.WriteEnd();
                    Debug.Assert(obj is HeapClosureHandle<TClosure, TEvent>);
                    return Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(obj);
                }
                HeapClosureHandle<TClosure, TEvent> handle = new HeapClosureHandle<TClosure, TEvent>();
                closureCallbacks[key] = handle;
                GetOrCreateSimpleHandle<TEvent>().AddValueClosure(handle);
                closureCallbacksLocker.WriteEnd();
                return handle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetClosureHandle<TClosure, TEvent>(out HeapClosureHandle<TClosure, TEvent> handle)
        {
            (Type, Type) key = (typeof(TClosure), typeof(TEvent));
            closureCallbacksLocker.ReadBegin();
            if (!closureCallbacks.TryGetValue(key, out object obj))
            {
                closureCallbacksLocker.ReadEnd();
                handle = null;
                    return false;
            }
            Debug.Assert(obj is HeapClosureHandle<TClosure, TEvent>);
            handle = Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(obj);
            closureCallbacksLocker.ReadEnd();
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");
    }
}