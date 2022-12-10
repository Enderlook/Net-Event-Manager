using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{
    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
    /// </summary>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        InvariantObject callback_ = callback.Wrap();
        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, No, InvariantObject>(callback_, listenToAssignableEvents);
        else
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, Yes, InvariantObject>(callback_, listenToAssignableEvents);
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
    /// </summary>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        InvariantObject callback_ = callback.Wrap();
        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, No, InvariantObject>(callback_, listenToAssignableEvents);
        else
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, Yes, InvariantObject>(callback_, listenToAssignableEvents);
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
    /// </summary>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & SubscribeFlags.RaiseOnce) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndT<TClosure?> callback_ = new(callback, closure);
            if (multipleRaises)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, No, InvariantObjectAndT<TClosure?>>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, Yes, InvariantObjectAndT<TClosure?>>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndT<object?> callback_ = new(callback, closure);
            if (multipleRaises)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndT<object?>>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndT<object?>>(callback_,  listenToAssignableEvents);
        }
    }

    /// <summary>
    /// Subscribes the callback <paramref name="callback"/> to execute when the event type <typeparamref name="TEvent"/> is raised.
    /// </summary>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & SubscribeFlags.RaiseOnce) == 0;
        if (typeof(TClosure).IsValueType)
        {
            InvariantObjectAndT<TClosure?> callback_ = new(callback, closure);
            if (multipleRaises)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, No, InvariantObjectAndT<TClosure?>>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, Yes, InvariantObjectAndT<TClosure?>>(callback_, listenToAssignableEvents);
        }
        else
        {
            InvariantObjectAndT<object?> callback_ = new(callback, closure);
            if (multipleRaises)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndT<object?>>(callback_, listenToAssignableEvents);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndT<object?>>(callback_, listenToAssignableEvents);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action{TEvent}, SubscribeFlags)"/>.
    /// </summary>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        InvariantObjectComparer<Action<TEvent>> predicator = new(callback);
        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, No, InvariantObjectComparer<Action<TEvent>>, InvariantObject>(predicator, listenToAssignableEvents);
        else
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, Yes, InvariantObjectComparer<Action<TEvent>>, InvariantObject>(predicator, listenToAssignableEvents);
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent}(Action, SubscribeFlags)"/>.
    /// </summary>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        InvariantObjectComparer<Action> predicator = new(callback);
        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, No, InvariantObjectComparer<Action>, InvariantObject>(predicator, listenToAssignableEvents);
        else
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, Yes, InvariantObjectComparer<Action>, InvariantObject>(predicator, listenToAssignableEvents);
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent, TClosure}(TClosure, Action{TClosure, TEvent}, SubscribeFlags)"/>.
    /// </summary>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & SubscribeFlags.RaiseOnce) == 0;
        if (typeof(TClosure).IsValueType)
        {
            StrongValueClosureActionComparer<TClosure?, TEvent> predicator = new(new(callback, closure));
            if (multipleRaises)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, No, StrongValueClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<TClosure?>>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, Yes, StrongValueClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<TClosure?>>(predicator, listenToAssignableEvents);
        }
        else
        {
            StrongReferenceClosureActionComparer<TClosure?, TEvent> predicator = new(new(callback, closure));
            if (multipleRaises)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, No, StrongReferenceClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<object?>>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, Yes, StrongReferenceClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<object?>>(predicator, listenToAssignableEvents);
        }
    }

    /// <summary>
    /// Unsubscribes a callback suscribed by <see cref="Subscribe{TEvent, TClosure}(TClosure, Action{TClosure}, SubscribeFlags)"/>.
    /// </summary>
    /// <param name="closure">Parameter to pass to <paramref name="callback"/>.</param>
    /// <param name="callback">Callback to no longer execute.</param>
    /// <param name="subscribeAttributes">Configures the callback subscription.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default)
    {
        if (callback is null) ThrowNullCallbackException();

        bool listenToAssignableEvents = (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0;
        bool multipleRaises = (subscribeAttributes & SubscribeFlags.RaiseOnce) == 0;
        if (typeof(TClosure).IsValueType)
        {
            StrongValueClosureActionComparer<TClosure?> predicator = new(new(callback, closure));
            if (multipleRaises)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, No, StrongValueClosureActionComparer<TClosure?>, InvariantObjectAndT<TClosure?>>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, Yes, StrongValueClosureActionComparer<TClosure?>, InvariantObjectAndT<TClosure?>>(predicator, listenToAssignableEvents);
        }
        else
        {
            StrongReferenceClosureActionComparer<TClosure?> predicator = new(new(callback, closure));
            if (multipleRaises)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, No, StrongReferenceClosureActionComparer<TClosure?>, InvariantObjectAndT<object?>>(predicator, listenToAssignableEvents);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, Yes, StrongReferenceClosureActionComparer<TClosure?>, InvariantObjectAndT<object?>>(predicator, listenToAssignableEvents);
        }
    }
}
