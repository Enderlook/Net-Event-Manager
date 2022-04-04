using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

/// <summary>
/// Represent a type safe event manager where types are used as events types.
/// </summary>
public sealed partial class EventManager : IDisposable
{
    // Key type is ICallbackExecuter<TEvent, TCallback>.
    // Value type is actually InvokersHolder<TEvent, TInvoke>.
    private Dictionary<Type, InvokersHolder> holdersPerType = new();

    // Key type is TEvent.
    // Value type is InvokersHolderManager<TEvent>.
    private Dictionary<Type, InvokersHolderManager> managersPerType = new();

    // Elements type derives from InvokersHolder.
    private InvariantObject[]? holders;
    private int holdersCount;

    /// <summary>
    /// A shared instance of the event manager.
    /// </summary>
    public static EventManager Shared { get; } = new EventManager();

    /// <summary>
    /// Automatically disposes the object in case it wasn't disposed by the user.
    /// </summary>
    ~EventManager() => Dispose();

    /// <summary>
    /// Raises an event type <typeparamref name="TEvent"/>, which executes all delegates subscribed to events of type <typeparamref name="TEvent"/> and to any other event type which is assignable from <typeparamref name="TEvent"/>.<br/>
    /// For example, given:
    /// <code>
    /// <see langword="interface"/> IEvent { }<br/>
    /// <see langword="class"/> BaseEvent : <see cref="object"/>, IEvent { }<br/>
    /// <see langword="class"/> ConcreteEvent : BaseEvent { }
    /// </code>
    /// <c>Raise&lt;ConcreteEvent&gt;(concreteEvent)</c> would run all subscribed delegates to events of type <c>ConcreteEvent</c>, <c>BaseEvent</c>, <c>IEvent</c> and <see cref="object"/>, which is equivalent to:
    /// <code>
    /// // Execution order is undefined.
    /// RaiseExactly&lt;ConcreteEvent&lt;(concreteEvent);
    /// RaiseExactly&lt;BaseEvent&lt;(concreteEvent);
    /// RaiseExactly&lt;IEvent&lt;(concreteEvent);
    /// RaiseExactly&lt;<see cref="object"/>&lt;(concreteEvent);
    /// </code>
    /// Execution order of subscribed delegates is undefined.<br/>
    /// If <typeparamref name="TEvent"/> is a value-type but assignable types are reference-types, the boxed instance is shared between all delegate calls (and may or may not be pooled).<br/>
    /// By convention, value-types should be immutable, so this is not a problem. However, if you have a mutable type, be warned.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="argument">Arguments of this event.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    public void RaiseHierarchy<TEvent>(TEvent argument)
    {
        ReadBegin();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromReadToInHolder();
                InvokersHolderManager<TEvent> manager_ = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
                manager_.RaiseHierarchy(argument, this);
            }
            else
                ReadEnd();
        }
    }

    /// <summary>
    /// Raises an event type <typeparamref name="TEvent"/>, which executes all delegates subscribed to events of type <typeparamref name="TEvent"/>.<br/>
    /// Execution order of subscribed delegates is undefined.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="argument">Arguments of this event.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    public void RaiseExactly<TEvent>(TEvent argument)
    {
        ReadBegin();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromReadToInHolder();
                InvokersHolderManager<TEvent> manager_ = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
                manager_.RaiseExactly(argument, this);
            }
            else
                ReadEnd();
        }
    }

    /// <summary>
    /// This is equivalent to <c><see cref="RaiseHierarchy{TEvent}(TEvent)"/></c> passing a <c><see langword="new"/> <typeparamref name="TEvent"/>()</c> instance as argument.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RaiseHierarchy<TEvent>() where TEvent : new()
        => RaiseHierarchy(new TEvent());

    /// <summary>
    /// This is equivalent to <c><see cref="RaiseExactly{TEvent}(TEvent)"/></c> passing a <c><see langword="new"/> <typeparamref name="TEvent"/>()</c> instance as argument.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RaiseExactly<TEvent>() where TEvent : new()
        => RaiseExactly(new TEvent());

    internal void Unsubscribe<TEvent, TCallbackHelper, TPredicator, TCallback>(TPredicator predicator)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
        where TPredicator : IPredicator<TCallback>
    {
        ReadBegin();
        {
            if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
            {
                FromReadToInHolder();
                {
                    Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder)
                        .Unsubscribe(predicator);
                }
                InHolderEnd();
            }
            else
                ReadEnd();
        }
    }

    internal void Subscribe<TEvent, TCallbackHelper, TCallback>(TCallback callback)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
    {
        ReadBegin();
        {
            if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
            {
                FromReadToInHolder();
                {
                    Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder).Subscribe(callback);
                }
                InHolderEnd();
            }
            else
                SlowPath();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SlowPath()
        {
            InvokersHolder<TEvent, TCallbackHelper, TCallback> holder_;
            FromReadToWrite();
            {
                if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
                {
                    holder_ = Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder);
                    goto exit;
                }
                else
                {
                    holder_ = new();
                    holdersPerType.Add(typeof(TCallbackHelper), holder_);

                    if (holders is null)
                    {
                        holders = ArrayUtils.RentArray<InvariantObject>(1);
                        AutoPurger _ = new(this);
                    }

                    ArrayUtils.Add(ref holders, ref holdersCount, new(holder_));

                    if (!managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? packHolder))
                        managersPerType.Add(typeof(TEvent), packHolder = new InvokersHolderManager<TEvent>());

                    InvokersHolderManager<TEvent> invokersHolderManager = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(packHolder);
                    invokersHolderManager.Add(holder_);

                    foreach (KeyValuePair<Type, InvokersHolderManager> kv in managersPerType)
                    {
                        if (kv.Key.IsAssignableFrom(typeof(TEvent)) && kv.Key != typeof(TEvent))
                            kv.Value.AddDerived(holder_, typeof(TEvent));
                        else if (typeof(TEvent).IsAssignableFrom(kv.Key) && kv.Key != typeof(TEvent))
                            invokersHolderManager.AddTo(kv.Value, kv.Key);
                    }
                }
            }
            exit:
            FromWriteToInHolder();
            {
                holder_.Subscribe(callback);
            }
            InHolderEnd();
        }
    }

    [DoesNotReturn]
    private static void ThrowNullCallbackException() => throw new ArgumentNullException("callback");

    [DoesNotReturn]
    private static void ThrowNullHandleException() => throw new ArgumentNullException("handle");

    [DoesNotReturn]
    private static void ThrowObjectDisposedException() => throw new ObjectDisposedException("Event Manager");
}
