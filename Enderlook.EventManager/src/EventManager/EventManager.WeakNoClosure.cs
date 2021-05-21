using System;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        // EventList<Action>
        private Handles<Type, Handle> weakWithoutParameter;
        // EventList<Action<TEvent>>
        private Handles<Type, Handle> weakWithParameter;
        // EventList<GCHandle, Action>
        private Handles<Type, Handle> weakWithHandleWithoutParameter;
        // EventList<GCHandle, Action<TEvent>>
        private Handles<Type, Handle> weakWithHandleWithParameter;
        // EventListOnce<Action>
        private Handles<Type, Handle> weakWithoutParameterOnce;
        // EventListOnce<Action<TEvent>>
        private Handles<Type, Handle> weakWithParameterOnce;
        // EventListOnce<GCHandle, Action>
        private Handles<Type, Handle> weakWithHandleWithoutParameterOnce;
        // EventListOnce<GCHandle, Action<TEvent>>
        private Handles<Type, Handle> weakWithHandleWithParameterOnce;

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithParameter
                .GetOrCreate<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithHandleWithParameter
                .GetOrCreate<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithoutParameter
                .GetOrCreate<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithHandleWithoutParameter
                .GetOrCreate<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Subscribes the callback <paramref name="callback"/> to execute only once when the event type <typeparamref name="TEvent"/> is raised.<br/>
        /// A weak reference to <paramref name="handle"/> is stored. If the reference becomes null, the callback is not executed.
        /// </summary>
        /// <param name="handle">Object whose weak reference will be stored.</param>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithParameterOnce
                .GetOrCreate<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithHandleWithParameterOnce
                .GetOrCreate<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithoutParameterOnce
                .GetOrCreate<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <inheritdoc cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{TEvent})"/>
        public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            weakWithHandleWithoutParameterOnce
                .GetOrCreate<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent>(ref @lock, handles)
                .Add(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (weakWithParameter.TryGet(out EventHandle<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle, TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class
        {
            if (callback is null)
                ThrowNullCallback();

            if (weakWithHandleWithParameter.TryGet(out EventHandle<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithHandleWithoutParameter.TryGet(out EventHandle<EventList<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{THandle, TEvent})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithHandleWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action)"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithoutHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref="WeakSubscribeOnce{THandle, TEvent}(THandle, Action{THandle})"/>.
        /// </summary>
        /// <param name="handle">Object whose weak reference was stored.</param>
        /// <param name="callback">Callback to no longer execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback)
            where THandle : class
        {
            if (handle is null)
                ThrowNullHandle();

            if (callback is null)
                ThrowNullCallback();

            if (weakWithHandleWithParameterOnce.TryGet(out EventHandle<EventListOnce<WeakDelegate>, WeakDelegate, HasNoClosureWeakWithHandle, Unused, TEvent> handle_))
                // TODO: we could avoid the GCHandle.Alloc (inside WeakDelegate constructor) when removing.
                handle_.Remove(new WeakDelegate(callback, handle));
        }
    }
}
