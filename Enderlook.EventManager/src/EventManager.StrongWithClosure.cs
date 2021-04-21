using System;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        // EventList<Action<object>>
        private Handles<Type, Handle> referenceClosureWithoutParameter;
        // EventList<Action<object, TEvent>>
        private Handles<Type, Handle> referenceClosureWithParameter;
        // EventListOnce<Action<object>>
        private Handles<Type, Handle> referenceClosureWithoutParameterOnce;
        // EventListOnce<Action<object, TEvent>>
        private Handles<Type, Handle> referenceClosureWithParameterOnce;
        // EventList<Action<TClosure>>
        private Handles<(Type, Type), Handle> valueClosureWithoutParameter;
        // EventList<Action<TClosure, TEvent>>
        private Handles<(Type, Type), Handle> valueClosureWithParameter;
        // EventListOnce<Action<TClosure>>
        private Handles<(Type, Type), Handle> valueClosureWithoutParameterOnce;
        // EventListOnce<Action<TClosure, TEvent>>
        private Handles<(Type, Type), Handle> valueClosureWithParameterOnce;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <inheritdoc cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithoutParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithoutParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <inheritdoc cref="SubscribeOnce{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (valueClosureWithParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (valueClosureWithoutParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithoutParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (valueClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TClosure, TEvent}(TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (valueClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosureStrong, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }
    }
}
