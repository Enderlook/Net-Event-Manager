using System;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        // EventList<Action>
        private Handles<Type, Handle> withoutParameter;
        // EventList<Action<TEvent>>
        private Handles<Type, Handle> withParameter;
        // EventListOnce<Action>
        private Handles<Type, Handle> withoutParameterOnce;
        // EventListOnce<Action<TEvent>>
        private Handles<Type, Handle> withParameterOnce;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Subscribe<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withParameter
                .GetOrCreate<EventList<Action<TEvent>>, Action<TEvent>, HasNoClosureStrong, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <inheritdoc cref="Subscribe{TEvent}(Action{TEvent})"/>
        public void Subscribe<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withoutParameter
                .GetOrCreate<EventList<Action>, Action, HasNoClosureStrong, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void SubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withParameterOnce
                .GetOrCreate<EventListOnce<Action<TEvent>>, Action<TEvent>, HasNoClosureStrong, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withoutParameterOnce
                .GetOrCreate<EventListOnce<Action>, Action, HasNoClosureStrong, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (withParameter.TryGet(out EventHandle<EventList<Action<TEvent>>, Action<TEvent>, HasNoClosureStrong, Unused, TEvent> handle))
                handle.Remove(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (withoutParameter.TryGet(out EventHandle<EventList<Action>, Action, HasNoClosureStrong, Unused, TEvent> handle))
                handle.Remove(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TEvent>(Action<TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (withParameterOnce.TryGet(out EventHandle<EventListOnce<Action<TEvent>>, Action<TEvent>, HasNoClosureStrong, Unused, TEvent> handle))
                handle.Remove(callback);
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (withParameterOnce.TryGet(out EventHandle<EventListOnce<Action>, Action, HasNoClosureStrong, Unused, TEvent> handle))
                handle.Remove(callback);
        }
    }
}
