using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.<br/>
    /// A weak reference to <paramref name="handle"/> is stored. If the reference is garbage collected, the callback is not executed.
    /// </summary>
    /// <param name="handle">Object whose weak reference will be stored.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{TEvent}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure, TEvent}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);

            }
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{TClosure}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure, TEvent}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TClosure, TEvent}(THandle, TClosure, Action{THandle, TClosure}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle, TEvent}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="WeakSubscribe{THandle, TEvent}(THandle, Action{THandle}, WeakSubscribeFlags)"/>.
    /// </summary>
    /// <param name="handle">Object whose weak reference was stored.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configuration that was used to subscribe the callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> or <paramref name="handle"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class
    {
        if (callback is null) ThrowNullCallbackException();
        if (handle is null) ThrowNullHandleException();

        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakMultipleCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakOnceCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }
}
