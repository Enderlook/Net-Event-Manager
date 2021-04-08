using System;
using System.Collections.Generic;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    /// <typeparam name="TEventBase">Base type of all events. Useful to determine a common ground.</typeparam>
    public sealed partial class EventManager<TEventBase>
    {
        private Dictionary<Type, TypeHandle> callbacks = new Dictionary<Type, TypeHandle>();

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Subscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                throw new ArgumentNullException(nameof(callback));

            Type key = typeof(TEvent);
            if (callbacks.TryGetValue(key, out TypeHandle handle))
            {
                handle.Suscribe(callback);
                callbacks[key] = handle;
            }
            else
                callbacks[key] = TypeHandle.Create(callback);
        }

        /// <inheritdoc cref="Subscribe{TEvent}(Action{TEvent})"/>
        public void Subscribe<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                throw new ArgumentNullException(nameof(callback));

            Type key = typeof(TEvent);
            if (callbacks.TryGetValue(key, out TypeHandle handle))
            {
                handle.Suscribe(callback);
                callbacks[key] = handle;
            }
            else
                callbacks[key] = TypeHandle.Create(callback);
        }

        /// <summary>
        /// Unsubscribes the callback <paramref name="callback"/> from execution when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
        {
            if (callback is null)
                throw new ArgumentNullException(nameof(callback));

            Type key = typeof(TEvent);
            if (callbacks.TryGetValue(key, out TypeHandle handle))
            {
                handle.Unsuscribe(callback);
                callbacks[key] = handle;
            }
        }

        /// <inheritdoc cref="Unsubscribe{TEvent}(Action{TEvent})"/>
        public void Unsubscribe<TEvent>(Action callback) where TEvent : TEventBase
        {
            if (callback is null)
                throw new ArgumentNullException(nameof(callback));

            Type key = typeof(TEvent);
            if (callbacks.TryGetValue(key, out TypeHandle handle))
            {
                handle.Unsuscribe(callback);
                callbacks[key] = handle;
            }
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument) where TEvent : TEventBase
        {
            if (callbacks.TryGetValue(typeof(TEvent), out TypeHandle handle))
                handle.Raise(eventArgument);
        }
    }
}