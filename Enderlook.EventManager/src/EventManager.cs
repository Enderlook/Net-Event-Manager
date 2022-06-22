﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

/// <summary>
/// Represent a type safe event manager where types are used as events types.
/// </summary>
public sealed partial class EventManager : IDisposable
{
    // TODO: We could add purging to these.
    private static int invokersHolderManagerCreatorsLock;
    private static int invokersHolderManagerCreatorsReaders;
    private static Dictionary2<Type, Func<EventManager, InvokersHolderManager>> invokersHolderManagerCreators;
    private static Type[]? one;

    // Value type is actually InvokersHolder<TEvent, TInvoke>.
    private Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType = new();

    // Key type is TEvent.
    // Value type is InvokersHolderManager<TEvent>.
    private Dictionary2<Type, InvokersHolderManager> managersPerType = new();

    /// <summary>
    /// A shared instance of the event manager.
    /// </summary>
    public static EventManager Shared { get; } = new EventManager();

    /// <summary>
    /// Raises an event type <typeparamref name="TEvent"/>.<br/>
    /// Execution order of subscribed delegates is undefined.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="argument">Arguments of this event.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    public void Raise<TEvent>(TEvent argument)
    {
        ReadBegin();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromReadToInHolder();
                InvokersHolderManager<TEvent> manager_ = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
                manager_.StaticRaise(argument, this);
            }
            else
                SlowPath();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SlowPath() => CreateInvokersHolderManager<TEvent>().StaticRaise(argument, this);
    }

    /// <summary>
    /// This is equivalent to <c><see cref="Raise{TEvent}(TEvent)"/></c> passing a <c><see langword="new"/> <typeparamref name="TEvent"/>()</c> instance as argument.<br/>
    /// Although, argument is only instantiated if there are subscribed delegates for that type.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Raise<TEvent>() where TEvent : new()
    {
        ReadBegin();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromReadToInHolder();
                InvokersHolderManager<TEvent> manager_ = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
                manager_.StaticRaise(new TEvent(), this);
            }
            else
                SlowPath();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SlowPath() => CreateInvokersHolderManager<TEvent>().StaticRaise(new TEvent(), this);
    }

    /// <summary>
    /// If <c><paramref name="argument"/> <see langword="is"/> <see langword="null"/></c>, it does the same as <see cref="Raise{TEvent}(TEvent)"/> passing <see langword="null"/>.<br/>
    /// If not, it runs the method <see cref="Raise{TEvent}(TEvent)"/> using the type of the passed instance as generic argument.<br/>
    /// This method is equivalent to following pseudo-code:
    /// <code>
    /// <see langword="if"/> (<paramref name="argument"/> <see langword="is"/> <see langword="null"/>)
    ///     RaiseExactly&lt;<typeparamref name="TEvent"/>&gt;(<see langword="default"/>);<br/>
    /// <see langword="else"/><br/>
    ///     RaiseExactly&lt;<paramref name="argument"/>.GetType()&gt;((<paramref name="argument"/>.GetType())<paramref name="argument"/>);
    /// </code>
    /// (Internally it doesn't use reflection unlike what the pseudo-code looks like, so it isn't expensive).
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="argument">Arguments of this event.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    public void DynamicRaise<TEvent>(TEvent argument)
    {
        Type key = argument?.GetType() ?? typeof(TEvent);
        ReadBegin();
        {
            if (managersPerType.TryGetValue(key, out InvokersHolderManager? manager))
            {
                FromReadToInHolder();
                // TODO: This virtual call is actually only required if argument.GetType().IsValueType
                // for reference-types we could use a direct call to StaticRaise if we tweaked a few debug assertions.
                manager.DynamicRaise(argument, this);
            }
            else
                SlowPath();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SlowPath() => CreateInvokersHolderManagerDynamic(key).DynamicRaise(argument, this);
    }

    internal void Unsubscribe<TEvent, TCallbackHelper, TPredicator, TCallback>(TPredicator predicator, bool listenToAssignableEvents)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
        where TPredicator : IPredicator<TCallback>
    {
        ReadBegin();
        {
            if (holdersPerType.TryGetValue(new(typeof(TCallbackHelper), listenToAssignableEvents), out InvokersHolder? holder))
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

    internal void Subscribe<TEvent, TCallbackHelper, TCallback>(TCallback callback, bool listenToAssignableEvents)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
    {
        ReadBegin();
        {
            if (holdersPerType.TryGetValue(new(typeof(TCallbackHelper), listenToAssignableEvents), out InvokersHolder? holder))
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
                ref InvokersHolder holderSlot = ref holdersPerType.GetOrCreateValueSlot(new(typeof(TCallbackHelper), listenToAssignableEvents), out bool found);
                if (found)
                {
                    holder_ = Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holderSlot);
                    goto exit;
                }
                else
                {
                    holder_ = new(listenToAssignableEvents);
                    holderSlot = holder_;

                    if (purgePhase == PurgePhase_FinalizerNotConfigured)
                    {
                        purgePhase = PurgePhase_PurgeInvokersHolder;
                        AutoPurger _ = new(this);
                    }

                    if (!managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
                    {
                        manager = new InvokersHolderManager<TEvent>();
                        if (managersPerType.Count > 0)
                        {
                            int index = 0;
                            while (managersPerType.MoveNext(ref index, out KeyValuePair<Type, InvokersHolderManager> kv))
                            {
                                if (kv.Key.IsAssignableFrom(typeof(TEvent)))
                                {
                                    Debug.Assert(kv.Key != typeof(TEvent));
                                    kv.Value.AddTo(manager, typeof(TEvent));
                                }
                            }
                        }
                        managersPerType.Add(typeof(TEvent), manager);
                    }

                    manager.Add(holder_);

                    if (listenToAssignableEvents && managersPerType.Count > 0)
                    {
                        int index = 0;
                        while (managersPerType.MoveNext(ref index, out KeyValuePair<Type, InvokersHolderManager> kv))
                        {
                            if (typeof(TEvent).IsAssignableFrom(kv.Key) && kv.Key != typeof(TEvent))
                                kv.Value.AddDerived(holder_, typeof(TEvent));
                        }
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    private InvokersHolderManager CreateInvokersHolderManagerDynamic(Type type)
    {
        // Get read lock.
        Lock(ref invokersHolderManagerCreatorsLock);
        invokersHolderManagerCreatorsReaders++;
        Unlock(ref invokersHolderManagerCreatorsLock);

        Func<EventManager, InvokersHolderManager>? creator;
        if (invokersHolderManagerCreators.TryGetValue(type, out creator))
        {
            // Release read lock.
            Lock(ref invokersHolderManagerCreatorsLock);
            invokersHolderManagerCreatorsReaders--;
            Unlock(ref invokersHolderManagerCreatorsLock);
        }
        else
        {
            // From read lock to write lock.
            Lock(ref invokersHolderManagerCreatorsLock);
            int invokersHolderManagerCreatorsReaders_ = --invokersHolderManagerCreatorsReaders;
            if (invokersHolderManagerCreatorsReaders_ != 0)
            {
                Unlock(ref invokersHolderManagerCreatorsLock);
                while (true)
                {
                    Lock(ref invokersHolderManagerCreatorsLock);
                    if (invokersHolderManagerCreatorsReaders > 0)
                        Unlock(ref invokersHolderManagerCreatorsLock);
                    else
                        break;
                }
            }

            ref Func<EventManager, InvokersHolderManager> value = ref invokersHolderManagerCreators.GetOrCreateValueSlot(type, out bool found);
            if (!found)
            {
                MethodInfo? methodInfo = typeof(EventManager).GetMethod(nameof(CreateInvokersHolderManager), BindingFlags.NonPublic | BindingFlags.Instance);
                Debug.Assert(methodInfo is not null);
                Type[] array = Interlocked.Exchange(ref one, null) ?? new Type[1];
                array[0] = type;
                MethodInfo methodInfoFull = methodInfo.MakeGenericMethod(array);
                array[0] = null!;
                one = array;
                value = creator = (Func<EventManager, InvokersHolderManager>)methodInfoFull.CreateDelegate(typeof(Func<EventManager, InvokersHolderManager>));
            }
            else
                creator = value;

            // Release write lock.
            Unlock(ref invokersHolderManagerCreatorsLock);
        }

        return creator(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private InvokersHolderManager<TEvent> CreateInvokersHolderManager<TEvent>()
    {
        InvokersHolderManager? manager;
        FromReadToWrite();
        {
            if (!managersPerType.TryGetValue(typeof(TEvent), out manager))
            {
                manager = new InvokersHolderManager<TEvent>();

                if (managersPerType.Count > 0)
                {
                    int index = 0;
                    while (managersPerType.MoveNext(ref index, out KeyValuePair<Type, InvokersHolderManager> kv))
                    {
                        if (kv.Key.IsAssignableFrom(typeof(TEvent)))
                        {
                            Debug.Assert(kv.Key != typeof(TEvent));
                            kv.Value.AddTo(manager, typeof(TEvent));
                        }
                    }
                }
                managersPerType.Add(typeof(TEvent), manager);
            }
        }
        FromWriteToInHolder();
        return Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
    }

    [DoesNotReturn]
    private static void ThrowNullCallbackException() => throw new ArgumentNullException("callback");

    [DoesNotReturn]
    private static void ThrowNullHandleException() => throw new ArgumentNullException("handle");

    [DoesNotReturn]
    private static void ThrowObjectDisposedException() => throw new ObjectDisposedException("Event Manager");
}
