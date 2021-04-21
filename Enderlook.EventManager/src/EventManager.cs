using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    public sealed class EventManager : IDisposable
    {
        private ReadWriterLock @lock;
        private Dictionary<Type, Handle> handles = new Dictionary<Type, Handle>();

        private Handles<Type, Handle> withoutParameter = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> withParameter = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> withoutParameterOnce = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> withParameterOnce = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> referenceClosureWithoutParameter = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> referenceClosureWithParameter = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> referenceClosureWithoutParameterOnce = Handles<Type, Handle>.Create();
        private Handles<Type, Handle> referenceClosureWithParameterOnce = Handles<Type, Handle>.Create();
        private Handles<(Type, Type), Handle> valueClosureWithoutParameter = Handles<(Type, Type), Handle>.Create();
        private Handles<(Type, Type), Handle> valueClosureWithParameter = Handles<(Type, Type), Handle>.Create();
        private Handles<(Type, Type), Handle> valueClosureWithoutParameterOnce = Handles<(Type, Type), Handle>.Create();
        private Handles<(Type, Type), Handle> valueClosureWithParameterOnce = Handles<(Type, Type), Handle>.Create();

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
                .GetOrCreate<EventList<Action<TEvent>>, Action<TEvent>, HasNoClosure, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <inheritdoc cref="Subscribe{TEvent}(Action{TEvent})"/>
        public void Subscribe<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withoutParameter
                .GetOrCreate<EventList<Action>, Action, HasNoClosure, Unused, TEvent>(ref @lock, handles)
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
                .GetOrCreate<EventListOnce<Action<TEvent>>, Action<TEvent>, HasNoClosure, Unused, TEvent>(ref @lock, handles)
                .Add(callback);
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TEvent>(Action callback)
        {
            if (callback is null)
                ThrowNullCallback();

            withoutParameterOnce
                .GetOrCreate<EventListOnce<Action>, Action, HasNoClosure, Unused, TEvent>(ref @lock, handles)
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

            if (withParameter.TryGet(out EventHandle<EventList<Action<TEvent>>, Action<TEvent>, HasNoClosure, Unused, TEvent> handle))
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

            if (withoutParameter.TryGet(out EventHandle<EventList<Action>, Action, HasNoClosure, Unused, TEvent> handle))
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

            if (withParameterOnce.TryGet(out EventHandle<EventListOnce<Action<TEvent>>, Action<TEvent>, HasNoClosure, Unused, TEvent> handle))
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

            if (withParameterOnce.TryGet(out EventHandle<EventListOnce<Action>, Action, HasNoClosure, Unused, TEvent> handle))
                handle.Remove(callback);
        }

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
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <inheritdoc cref="Subscribe{TClosure, TEvent}(TClosure, Action{TClosure, TEvent})"/>
        public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithoutParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithoutParameter
                    .GetOrCreate<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
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
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <inheritdoc cref="SubscribeOnce{TEvent}(Action{TEvent})"/>
        public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback)
        {
            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                valueClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
            else
                referenceClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent>(ref @lock, handles)
                    .Add(new ClosureDelegate<TClosure>(callback, closure));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent})"/>.
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
                if (valueClosureWithParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action)"/>.
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
                if (valueClosureWithoutParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithoutParameter.TryGet(out EventHandle<EventList<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
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
                if (valueClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
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
                if (valueClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
            else
            {
                if (referenceClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<ClosureDelegate<TClosure>>, ClosureDelegate<TClosure>, HasClosure, TClosure, TEvent> handle))
                    handle.Remove(new ClosureDelegate<TClosure>(callback, closure));
            }
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument)
        {
            @lock.ReadBegin();
            if (handles.TryGetValue(typeof(TEvent), out Handle handle))
            {
                @lock.ReadEnd();
                Debug.Assert(handle is GlobalHandle<TEvent>);
                Unsafe.As<GlobalHandle<TEvent>>(handle).Raise(eventArgument);
            }
            else
                @lock.ReadEnd();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            @lock.ReadBegin();
            foreach (Handle handle in handles.Values)
                handle.Dispose();
            @lock.ReadEnd();
        }

        /// <summary>
        /// Force the purge of removed delegates.
        /// </summary>
        public void Purge()
        {
            @lock.ReadBegin();
            foreach (Handle handle in handles.Values)
                handle.CompactAndPurge();
            @lock.ReadEnd();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");
    }
}