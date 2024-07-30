using Enderlook.Threading.Primitives;

using System;
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
    private static SpinLockSlim invokersHolderManagerCreatorsLock;
    private static int invokersHolderManagerCreatorsState;
    private static SpinLockSlim invokersHolderManagerCreatorsStateLock;
    private static int invokersHolderManagerCreatorsPurgeIndex;
    private static Dictionary2<Type, (Func<EventManager, InvokersHolderManager>? Delegate, int MillisecondsTimestamp)> invokersHolderManagerCreators;
    private static Type[]? one;

    // Value type is actually InvokersHolder<TEvent, TInvoke>.
    private Dictionary2<InvokersHolderTypeKey, InvokersHolder> holdersPerType = new();

    // Key type is TEvent.
    // Value type is InvokersHolderManager<TEvent>.
    private Dictionary2<Type, InvokersHolderManager> managersPerType = new();

    /// <summary>
    /// A shared instance of the event manager.
    /// </summary>
    public static EventManager Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Container.Shared; // Avoid having an static constructor in a public type.
    }

    private static class Container
    {
        public static readonly EventManager Shared = new();
    }

    /// <summary>
    /// Raises an event type <typeparamref name="TEvent"/>.<br/>
    /// Execution order of subscribed delegates is undefined.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="argument">Arguments of this event.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    public void Raise<TEvent>(TEvent argument)
    {
        Lock();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromLockToInHolder();
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
    public void Raise<TEvent>() where TEvent : new()
    {
        Lock();
        {
            if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
            {
                FromLockToInHolder();
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
        Lock();
        {
            if (managersPerType.TryGetValue(key, out InvokersHolderManager? manager))
            {
                FromLockToInHolder();
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

    internal void Unsubscribe<TEvent, TCallbackHelper, TIsOnce, TPredicator, TCallback>(TPredicator predicator, bool listenToAssignableEvents)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
        where TPredicator : IPredicator<TCallback>
    {
        Lock();
        {
            if (holdersPerType.TryGetValue(new(typeof(TCallbackHelper), listenToAssignableEvents), out InvokersHolder? holder))
            {
                FromLockToInHolder();
                {
                    Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce>>(holder)
                        .Unsubscribe(predicator);
                }
                InHolderEnd();
            }
            else
                ReadEnd();
        }
    }

    internal void Subscribe<TEvent, TCallbackHelper, TIsOnce, TCallback>(TCallback callback, bool listenToAssignableEvents)
        where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
    {
        Lock();
        {
            if (holdersPerType.TryGetValue(new(typeof(TCallbackHelper), listenToAssignableEvents), out InvokersHolder? holder))
            {
                FromLockToInHolder();
                {
                    Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce>>(holder).Subscribe(callback);
                }
                InHolderEnd();
            }
            else
                SlowPath();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SlowPath()
        {
            InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce> holder_;
            ref InvokersHolder holderSlot = ref holdersPerType.GetOrCreateValueSlot(new(typeof(TCallbackHelper), listenToAssignableEvents), out bool found);
            if (found)
            {
                holder_ = Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce>>(holderSlot);
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

                Dictionary2<Type, InvokersHolderManager> managersPerType = this.managersPerType;
                if (!managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
                {
                    manager = new InvokersHolderManager<TEvent>();
                    if (managersPerType.Count > 0)
                    {
                        int index = 0;
                        while (managersPerType.MoveNext(ref index, out Type? key, out InvokersHolderManager? value))
                        {
                            if (key.IsAssignableFrom(typeof(TEvent)))
                            {
                                Debug.Assert(key != typeof(TEvent));
                                value.AddTo(manager, typeof(TEvent));
                            }
                        }
                    }
                    managersPerType.Add(typeof(TEvent), manager);
                }

                manager.Add(holder_);

                if (listenToAssignableEvents && managersPerType.Count > 0)
                {
                    int index = 0;
                    while (managersPerType.MoveNext(ref index, out Type? key, out InvokersHolderManager? value))
                    {
                        if (typeof(TEvent).IsAssignableFrom(key) && key != typeof(TEvent))
                            value.AddDerived(holder_, typeof(TEvent));
                    }
                }
                this.managersPerType = managersPerType;
            }
        exit:
            FromLockToInHolder();
            {
                holder_.Subscribe(callback);
            }
            InHolderEnd();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private InvokersHolderManager CreateInvokersHolderManagerDynamic(Type type)
    {
        int invokersHolderManagerCreatorsState_ = Volatile.Read(ref invokersHolderManagerCreatorsState);
        if (invokersHolderManagerCreatorsState_ != 0)
        {
            // Request cancellation.
            SpinWait spinWait = new();
            while ((invokersHolderManagerCreatorsState_ & IS_PURGING) == 0)
            {
                int oldState = Interlocked.CompareExchange(ref invokersHolderManagerCreatorsState, invokersHolderManagerCreatorsState_ | IS_CANCELLATION_REQUESTED, invokersHolderManagerCreatorsState_);
                if (oldState != invokersHolderManagerCreatorsState_)
                {
                    invokersHolderManagerCreatorsState_ = oldState;
                    spinWait.SpinOnce();
                }
            }
        }

        bool enter = false;
        ref (Func<EventManager, InvokersHolderManager>? Delegate, int MillisecondsTimestamp) value = ref Utils.NullRef<(Func<EventManager, InvokersHolderManager>? Delegate, int MillisecondsTimestamp)>();
        try
        {
            invokersHolderManagerCreatorsLock.Enter(ref enter);
            value = ref invokersHolderManagerCreators.GetOrCreateValueSlot(type, out _);
        }
        finally
        {
            if (enter)
                invokersHolderManagerCreatorsLock.Exit();
        }

        Func<EventManager, InvokersHolderManager>? creator = value.Delegate;

        // Atomic swap.
        // Multiple threads can set it at the same time, but it's just an timestamp, so we don't worry about race conditions.
        value.MillisecondsTimestamp = Environment.TickCount;

        if (creator is null)
        {
            MethodInfo? methodInfo = typeof(EventManager).GetMethod(nameof(CreateInvokersHolderManager), BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(methodInfo is not null);
            Type[] array = Interlocked.Exchange(ref one, null) ?? new Type[1];
            array[0] = type;
            MethodInfo methodInfoFull = methodInfo.MakeGenericMethod(array);
            array[0] = null!;
            one = array;
            creator = (Func<EventManager, InvokersHolderManager>)methodInfoFull.CreateDelegate(typeof(Func<EventManager, InvokersHolderManager>));
            Func<EventManager, InvokersHolderManager>? old = Interlocked.CompareExchange(ref value.Delegate, creator, null);
            creator = old ?? creator;
        }

        return creator(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private InvokersHolderManager<TEvent> CreateInvokersHolderManager<TEvent>()
    {
        InvokersHolderManager? manager;
        Dictionary2<Type, InvokersHolderManager> managersPerType = this.managersPerType;
        if (!managersPerType.TryGetValue(typeof(TEvent), out manager))
        {
            manager = new InvokersHolderManager<TEvent>();

            if (managersPerType.Count > 0)
            {
                int index = 0;
                while (managersPerType.MoveNext(ref index, out Type? key, out InvokersHolderManager? value))
                {
                    if (key.IsAssignableFrom(typeof(TEvent)))
                    {
                        Debug.Assert(key != typeof(TEvent));
                        value.AddTo(manager, typeof(TEvent));
                    }
                }
            }
            managersPerType.Add(typeof(TEvent), manager);
        }
        this.managersPerType = managersPerType;
        FromLockToInHolder();
        return Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
    }

    [DoesNotReturn]
    private static void ThrowNullCallbackException() => throw new ArgumentNullException("callback");

    [DoesNotReturn]
    private static void ThrowNullHandleException() => throw new ArgumentNullException("handle");

    [DoesNotReturn]
    private static void ThrowObjectDisposedException() => throw new ObjectDisposedException("Event Manager");

    [DoesNotReturn]
    private void RunAndThrowObjectDisposedException<TCallback>()
        where TCallback : struct, ICallback
    {
#if NET7_0_OR_GREATER
        TCallback.Invoke(this);
#else
        new TCallback().Invoke(this);
#endif
        throw new ObjectDisposedException("Event Manager");
    }
}