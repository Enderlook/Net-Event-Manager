using System;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        // EventList<Action<object>>
        private Handles<Type, Handle> weakReferenceClosureWithoutParameter;
        // EventList<Action<object, TEvent>>
        private Handles<Type, Handle> weakReferenceClosureWithParameter;
        // EventList<Action<GcHandle, object>>
        private Handles<Type, Handle> weakWithHandleReferenceClosureWithoutParameter;
        // EventList<Action<GcHandle, object, TEvent>>
        private Handles<Type, Handle> weakWithHandleReferenceClosureWithParameter;
        // EventListOnce<Action<object>>
        private Handles<Type, Handle> weakReferenceClosureWithoutParameterOnce;
        // EventListOnce<Action<object, TEvent>>
        private Handles<Type, Handle> weakReferenceClosureWithParameterOnce;
        // EventListOnce<Action<GcHandle, object>>
        private Handles<Type, Handle> weakWithHandleReferenceClosureWithoutParameterOnce;
        // EventListOnce<Action<GcHandle, object, TEvent>>
        private Handles<Type, Handle> weakWithHandleReferenceClosureWithParameterOnce;

        // EventList<Action<TClosure>>
        private Handles<(Type, Type), Handle> weakValueClosureWithoutParameter;
        // EventList<Action<TClosure, TEvent>>
        private Handles<(Type, Type), Handle> weakValueClosureWithParameter;
        // EventList<Action<GCHandle, TClosure>>
        private Handles<(Type, Type), Handle> weakWithHandleValueClosureWithoutParameter;
        // EventList<Action<GCHandle, TClosure, TEvent>>
        private Handles<(Type, Type), Handle> weakWithHandleValueClosureWithParameter;
        // EventListOnce<Action<TClosure>>
        private Handles<(Type, Type), Handle> weakValueClosureWithoutParameterOnce;
        // EventListOnce<Action<TClosure, TEvent>>
        private Handles<(Type, Type), Handle> weakValueClosureWithParameterOnce;
        // EventListOnce<Action<GCHandle, TClosure>>
        private Handles<(Type, Type), Handle> weakWithHandleValueClosureWithoutParameterOnce;
        // EventListOnce<Action<GCHandle, TClosure, TEvent>>
        private Handles<(Type, Type), Handle> weakWithHandleValueClosureWithParameterOnce;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakValueClosureWithParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakReferenceClosureWithParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakWithHandleValueClosureWithParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakWithHandleReferenceClosureWithParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakValueClosureWithoutParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakReferenceClosureWithoutParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakWithHandleValueClosureWithoutParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakWithHandleReferenceClosureWithoutParameter
                    .GetOrCreate<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakValueClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakReferenceClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakWithHandleValueClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakWithHandleReferenceClosureWithParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakValueClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakReferenceClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent})"/>
        public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
                weakWithHandleValueClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
            else
                weakWithHandleReferenceClosureWithoutParameterOnce
                    .GetOrCreate<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent>(ref @lock, handles)
                    .Add(new WeakDelegate<TClosure>(callback, handle, closure));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle, TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakValueClosureWithParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakReferenceClosureWithParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle, TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakWithHandleValueClosureWithParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakWithHandleReferenceClosureWithParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakValueClosureWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakReferenceClosureWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakWithHandleValueClosureWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakWithHandleReferenceClosureWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakValueClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakReferenceClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action{TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakWithHandleValueClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakWithHandleReferenceClosureWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakValueClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakReferenceClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithoutHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="SubscribeOnce{TEvent}(Action)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (typeof(TClosure).IsValueType)
            {
                if (weakWithHandleValueClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
            else
            {
                if (weakWithHandleReferenceClosureWithoutParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate<TClosure>>, WeakDelegate<TClosure>, HasNoClosureWeakWithHandle, TClosure, TEvent> handle_))
                    handle_.Remove(new WeakDelegate<TClosure>(callback, handle, closure));
            }
        }
    }
}
