using System;
using System.Collections.Generic;
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
        // `object` is actually `TypeHandle<TEvent>`.
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

            GetOrCreateTypeHandler<TEvent>().Subscribe(callback);
        }

        /// <inheritdoc cref="Subscribe{TEvent}(Action{TEvent})"/>
        public void Subscribe<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateTypeHandler<TEvent>().Subscribe(callback);
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

            GetOrCreateTypeHandler<TEvent>().SubscribeOnce(callback);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            GetOrCreateTypeHandler<TEvent>().SubscribeOnce(callback);
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

            if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
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

            if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
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

            if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
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

            if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
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

            if (!typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TEvent>().Subscribe(callback, closure);
            else
                GetOrCreateClosureHandler<TClosure, TEvent>().handle.Subscribe(callback, closure);
        }

        /// <inheritdoc cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (!typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TEvent>().Subscribe(callback, closure);
            else
                GetOrCreateClosureHandler<TClosure, TEvent>().handle.Subscribe(callback, closure);
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

            if (!typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TEvent>().Subscribe(callback, closure);
            else
                GetOrCreateClosureHandler<TClosure, TEvent>().handle.Subscribe(callback, closure);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
        {
            if (callback is null)
                ThrowNullCallback();

            if (!typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TEvent>().Subscribe(callback, closure);
            else
                GetOrCreateClosureHandler<TClosure, TEvent>().handle.Subscribe(callback, closure);
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

            if (!typeof(TClosure).IsValueType)
            {
                if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
            else
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
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

            if (!typeof(TClosure).IsValueType)
            {
                if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
            else
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
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

            if (!typeof(TClosure).IsValueType)
            {
                if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
            else
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
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

            if (!typeof(TClosure).IsValueType)
            {
                if (TryGetTypeHandle(out TypeHandle<TEvent> handle))
                    handle.Unsubscribe(callback, closure);
            }
            else
            {
                if (TryGetClosureHandle(out HeapClosureHandle<TClosure, TEvent> handle))
                    handle.handle.Subscribe(callback, closure);
            }
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument) where TEvent : TEventBase
        {
            if (TryGetTypeHandle(out TypeHandle<TEvent> simpleHandler))
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
                    foreach (TypeHandle handle in simpleCallbacks.Values)
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
                    foreach (TypeHandle handle in simpleCallbacks.Values)
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
        private TypeHandle<TEvent> GetOrCreateTypeHandler<TEvent>()
        {
            Type key = typeof(TEvent);

            simpleCallbacksLocker.ReadBegin();
            try
            {
                if (simpleCallbacks.TryGetValue(key, out object obj))
                    return Unsafe.As<TypeHandle<TEvent>>(obj);
            }
            finally
            {
                simpleCallbacksLocker.ReadEnd();
            }

            simpleCallbacksLocker.WriteBegin();
            try
            {
                if (simpleCallbacks.TryGetValue(key, out object obj))
                    return Unsafe.As<TypeHandle<TEvent>>(obj);
                TypeHandle<TEvent> handle = new TypeHandle<TEvent>();
                simpleCallbacks[key] = handle;
                return handle;
            }
            finally
            {
                simpleCallbacksLocker.WriteEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetTypeHandle<TEvent>(out TypeHandle<TEvent> handle)
        {
            Type key = typeof(TEvent);
            simpleCallbacksLocker.ReadBegin();
            try
            {
                if (!simpleCallbacks.TryGetValue(key, out object obj))
                {
                    handle = null;
                    return false;
                }
                handle = Unsafe.As<TypeHandle<TEvent>>(obj);
                return true;
            }
            finally
            {
                simpleCallbacksLocker.ReadEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HeapClosureHandle<TClosure, TEvent> GetOrCreateClosureHandler<TClosure, TEvent>()
        {
            (Type, Type) key = (typeof(TClosure), typeof(TEvent));

            closureCallbacksLocker.ReadBegin();
            try
            {
                if (closureCallbacks.TryGetValue(key, out object handle))
                    return Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(handle);
            }
            finally
            {
                closureCallbacksLocker.ReadEnd();
            }

            closureCallbacksLocker.WriteBegin();
            try
            {
                if (closureCallbacks.TryGetValue(key, out object obj))
                    return Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(obj);

                HeapClosureHandle<TClosure, TEvent> handle = new HeapClosureHandle<TClosure, TEvent>();
                closureCallbacks[key] = handle;
                GetOrCreateTypeHandler<TEvent>().AddValueClosure(handle);
                return handle;
            }
            finally
            {
                closureCallbacksLocker.WriteEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetClosureHandle<TClosure, TEvent>(out HeapClosureHandle<TClosure, TEvent> handle)
        {
            (Type, Type) key = (typeof(TClosure), typeof(TEvent));
            closureCallbacksLocker.ReadBegin();
            try
            {
                if (!closureCallbacks.TryGetValue(key, out object obj))
                {
                    handle = null;
                    return false;
                }
                handle = Unsafe.As<HeapClosureHandle<TClosure, TEvent>>(obj);
                return true;
            }
            finally
            {
                closureCallbacksLocker.ReadEnd();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");
    }
}