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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            InvariantObjectAndGCHandle callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndGCHandleTrackResurrection callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            InvariantObjectAndGCHandle callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndGCHandleTrackResurrection callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
            }
        }
        else
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
            }
        }
        else
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
            }
        }
        else
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<TClosure?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(callback_, listenToAssignableEvents);
            }
        }
        else
        {
            if (trackResurrection)
            {
                InvariantObjectAndTAndGCHandle<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandle<object?>>(callback_, listenToAssignableEvents);
            }
            else
            {
                InvariantObjectAndTAndGCHandleTrackResurrection<object?> callback_ = new(callback, handle, closure);
                if (multipleRaises)
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
                else
                    Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            InvariantObjectAndGCHandle callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, No, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, Yes, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndGCHandleTrackResurrection callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            InvariantObjectAndGCHandle callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, No, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, Yes, InvariantObjectAndGCHandle>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndGCHandleTrackResurrection callback_ = new(callback, handle);
            if (multipleRaises)
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, No, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleTrackResurrection>(callback_, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle> predicator = new(callback, handle);
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle> predicator = new(callback, handle);
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
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

        InvariantObjectAndGCHandleComparer<Action, THandle> predicator = new(callback, handle);
        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
        }
        else
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, No, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
            }
        }
        else
        {
            InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TClosure?, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
            }
        }
        else
        {
            InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TClosure?, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
            }
        }
        else
        {
            InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
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

        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        bool trackResurrection = (subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(predicator, listenToAssignableEvents);
            }
        }
        else
        {
            InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle> predicator = new(callback, closure, handle);
            if (trackResurrection)
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(predicator, listenToAssignableEvents);
            }
            else
            {
                if (multipleRaises)
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, No, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
                else
                    Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TClosure?, THandle, TEvent>>, Yes, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(predicator, listenToAssignableEvents);
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

        InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle> predicator = new(callback, handle);
        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
        }
        else
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
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

        InvariantObjectAndGCHandleComparer<Action<THandle>, THandle> predicator = new(callback, handle);
        bool listenToAssignableEvents = (subscribeAttributes & WeakSubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & WeakSubscribeFlags.RaiseOnce) == 0;
        if ((subscribeAttributes & WeakSubscribeFlags.TrackResurrection) == 0)
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(predicator, listenToAssignableEvents);
        }
        else
        {
            if (multipleRaises)
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, No, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, WeakCallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, Yes, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(predicator, listenToAssignableEvents);
        }
    }
}
