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
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<TEvent>>, No, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<TEvent>>, Yes, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<TEvent>>, No, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<TEvent>>, Yes, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            }
            else
            {
                if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
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
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle), (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0);
        }
    }
}
