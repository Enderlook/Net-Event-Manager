![.NET Core](https://github.com/Enderlook/Net-Event-Manager/workflows/.NET%20Core/badge.svg?branch=master)

# .NET Event Manager

A type safe event manager library for .NET.  
Due the use of generic, this event manager doesn't suffer for boxing and unboxing of value types nor for casting errors of the consumers.  
Additionaly, closures of delegates can be stored apart in order to reuse the delegate and reduce allocations (i.e: `Action<T>` instead of `Action`, so you can pass a value that works as closure).  
Also, it support and respect (if configured to do so) inheritance of event types which can be used to categorize events by hierarchy. For example, if you raise an event of type `ConcreteEvent`, both delegates subscribed to `ConcreteEvent` **and** `BaseEvent` are run (and `IEvent` if it does implement it). For receiving **all** events, just subscribe to `Object` and configure that specific subscription to listen to all assignable types.  
Even more, it support raising event dynamically when the type is not know at compile-time.  
Finally, it has support for weak-refence listeners.

The following example show some of the functions of the event manager:
```cs
public static class Player
{
    private static EventManager eventManager = new EventManager();

    public static void Main()
    {
        eventManager.Subscribe<PlayerHurtEvent>("Ouch", OnPlayerHurt);

        eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem);

        eventManager.Subscribe<PlayerPickedUpItemEvent>(("Excalibur", "Mimic"), OnPlayerPickedUpItem2);
  
        eventManager.Subscribe<object>(OnAnyEvent, SubscribeFlags.ListenAssignableEvents);

        eventManager.Raise(new PlayerPickedUpItemEvent("Excalibur"));

        eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItemOnce, SubscribeFlags.RaiseOnce);

        eventManager.Raise(new PlayerPickedUpItemEvent("Pencil"));

        eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));

        eventManager.Unsubscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem2);

        eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));

        object obj = new PlayerPickedUpItemEvent("Mimic");
        eventManager.DynamicRaise(obj); // Note that we assign `object` instead of `PlayerPickedUpItemEvent`.
    }

    private static void OnPlayerHurt(string closure)
        => Console.WriteLine(closure);

    private static void OnPlayerPickedUpItem(PlayerPickedUpItemEvent @event)
        => Console.WriteLine($"Picked {@event.Item}");

    private static void OnPlayerPickedUpItem2((string, string) closure, PlayerPickedUpItemEvent @event)
    {
        if (@event.Item == closure.Item1)
            Console.WriteLine("Player picked up legendary sword!");
        else if (@event.Item == closure.Item2)
        {
            Console.WriteLine("Oh no! You picked up a mimic!");
            eventManager.RaiseExactly(new PlayerHurtEvent());
        }
    }

    private static void OnPlayerPickedUpItemOnce(PlayerPickedUpItemEvent @event)
        => Console.WriteLine($"This was registered to only execute once.");

   private static void OnAnyEvent(object @event)
        => Console.WriteLine(@event.GetType());

    public readonly struct PlayerHurtEvent { }

    public readonly struct PlayerPickedUpItemEvent
    {
        public readonly string Item;

        public PlayerPickedUpItemEvent(string item) => Item = item;
    }
}
```

## API

```cs
/// Type safe event manager where each type represent an event type.
public sealed class EventManager : IDisposable
{
    /// Subscribes an action to run when the event `TEvent` is raised.
    public void Subscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Subscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Unsubscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Unsubscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);

    /// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);

    /// Subscribes an action to run when the event `TEvent` is raised.
    /// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;

    /// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
    /// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;
    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default)
        where THandle : class;

    /// Raise the event of type `TEvent`.
    public void Raise<TEvent>(TEvent eventArgument);

    /// Raise the event of type `TEvent` with a new instance of `TEvent` using its parameterless constructor.
    public void Raise<TEvent>() where TEvent : new();

    /// Raise the event of the type `eventArgument.GetType()` or `typeof(TEvent)` if `eventArgument is null`.
    public void DynamicRaise<TEvent>(TEvent eventArgument);

    /// Dispose the underlying content of this event manager.
    public void Dispose();
}

/// Determines the configuration of the delegate to subscribe.
[Flags]
public enum SubscribeFlags
{
    /// By default callbacks can be executed multiple times and only listen to the exact type match.
    /// This behaviour can be overriden by applying other flags.
    Default = 0,

    /// The callback is automatically unsubscribed from the event manager after its first execution.
    RaiseOnce,

    /// The callback will listen to any event whose type assignable to the event type of this delegate
    /// For example, a delegate `Action<BaseEvent>` will not only listen for type `BaseEvent` but also for `ConcreteEvent` given `ConcreteEvent` is a subtype of `BaseEvent`.
    /// This also supports interface types.
    /// When the derived type is a value-type and the callback takes a reference-type (i.e: `Action<object> and derived type `SomeEvent` is a `struct`), the boxed event is shared to all delegate calls (and may or may not be pooled).
    /// By convention, value-types should be immutable, so this is not a problem. However, if you have a mutable type, be warned.
    ListenAssignableEvents,
}

/// Determines the configuration of the delegate to subscribe.
[Flags]
public enum WeakSubscribeFlags
{
    /// By default callbacks can be executed multiple times, are only listen to the exact type match and resurrection of handle is not tracked.
    /// This behaviour can be overriden by applying other flags.
    Default = 0,

    /// The callback is automatically unsubscribed from the event manager after its first execution.
    RaiseOnce,

    /// The callback will listen to any event whose type assignable to the event type of this delegate
    /// For example, a delegate `Action<BaseEvent>` will not only listen for type `BaseEvent` but also for `ConcreteEvent` given `ConcreteEvent` is a subtype of `BaseEvent`.
    /// This also supports interface types.
    /// When the derived type is a value-type and the callback takes a reference-type (i.e: `Action<object> and derived type `SomeEvent` is a `struct`), the boxed event is shared to all delegate calls (and may or may not be pooled).
    /// By convention, value-types should be immutable, so this is not a problem. However, if you have a mutable type, be warned.
    ListenAssignableEvents,

    /// Includes tracking the resurrection of the handle.
    TrackResurrection,
}
```
