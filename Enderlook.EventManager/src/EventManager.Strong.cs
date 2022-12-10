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

        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, No, InvariantObject>(callback.Wrap(), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        else
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, Yes, InvariantObject>(callback.Wrap(), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, No, InvariantObject>(callback.Wrap(), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        else
            Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, Yes, InvariantObject>(callback.Wrap(), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, No, InvariantObjectAndT<TClosure?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, Yes, InvariantObjectAndT<TClosure?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, No, InvariantObjectAndT<object?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, Yes, InvariantObjectAndT<object?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, No, InvariantObjectAndT<TClosure?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, Yes, InvariantObjectAndT<TClosure?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, No, InvariantObjectAndT<object?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Subscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, Yes, InvariantObjectAndT<object?>>(new(callback, closure), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, No, InvariantObjectComparer<Action<TEvent>>, InvariantObject>(new(callback.Wrap()), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        else
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, Yes, InvariantObjectComparer<Action<TEvent>>, InvariantObject>(new(callback.Wrap()), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, No, InvariantObjectComparer<Action>, InvariantObject>(new(callback.Wrap()), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        else
            Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, Yes, InvariantObjectComparer<Action>, InvariantObject>(new(callback.Wrap()), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, No, StrongValueClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, Yes, StrongValueClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, No, StrongReferenceClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<object?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, Yes, StrongReferenceClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<object?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
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

        if (typeof(TClosure).IsValueType)
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, No, StrongValueClosureActionComparer<TClosure?>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, Yes, StrongValueClosureActionComparer<TClosure?>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        }
        else
        {
            if ((subscribeAttributes & SubscribeFlags.RaiseOnce) == 0)
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, No, StrongReferenceClosureActionComparer<TClosure?>, InvariantObjectAndT<object?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
            else
                Unsubscribe<TEvent, StrongCallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, Yes, StrongReferenceClosureActionComparer<TClosure?>, InvariantObjectAndT<object?>>(new(new(callback, closure)), (subscribeAttributes & SubscribeFlags.ListenAssignableEvents) != 0);
        }
    }
}
