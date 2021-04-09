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
        private ReadWriterLock locker;
        private Dictionary<Type, TypeHandle> callbacks = new Dictionary<Type, TypeHandle>();
        private Delegate[] a = Array.Empty<Delegate>();
        private Delegate[] b = Array.Empty<Delegate>();

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

            if (TryGetTypeHandle<TEvent>(out TypeHandle handle))
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

            if (TryGetTypeHandle<TEvent>(out TypeHandle handle))
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

            if (TryGetTypeHandle<TEvent>(out TypeHandle handle))
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

            if (TryGetTypeHandle<TEvent>(out TypeHandle handle))
                handle.UnsubscribeOnce(callback);
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument) where TEvent : TEventBase
        {
            bool found;
            TypeHandle handle;
            locker.ReadBegin();
            try
            {
                found = callbacks.TryGetValue(typeof(TEvent), out handle);
            }
            finally
            {
                locker.ReadEnd();
            }
            if (found)
                handle.Raise(eventArgument, ref a, ref b);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            locker.WriteBegin();
            try
            {
                foreach (TypeHandle handle in callbacks.Values)
                    handle.Dispose();
                callbacks.Clear();
            }
            finally
            {
                locker.WriteEnd();
            }
        }

        /// <summary>
        /// Force the purge of dead weak delegates.
        /// </summary>
        public void Purge()
        {
            locker.ReadBegin();
            try
            {
                foreach (TypeHandle handle in callbacks.Values)
                    handle.Purge(ref a, ref b);
            }
            finally
            {
                locker.ReadEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypeHandle GetOrCreateTypeHandler<TEvent>()
        {
            Type key = typeof(TEvent);
            locker.ReadBegin();
            bool found;
            TypeHandle handle;
            try
            {
                found = callbacks.TryGetValue(key, out handle);
            }
            finally
            {
                locker.ReadEnd();
            }
            if (found)
                return handle;
            else
            {
                handle = new TypeHandle();
                locker.WriteBegin();
                try
                {
                    callbacks[key] = handle;
                }
                finally
                {
                    locker.WriteEnd();
                }
                return handle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetTypeHandle<TEvent>(out TypeHandle handle)
        {
            bool found;
            Type key = typeof(TEvent);
            locker.ReadBegin();
            try
            {
                found = callbacks.TryGetValue(key, out handle);
            }
            finally
            {
                locker.ReadEnd();
            }
            return found;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowNullCallback() => throw new ArgumentNullException("callback");
    }
}