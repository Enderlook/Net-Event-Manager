![.NET Core](https://github.com/Enderlook/Net-Event-Manager/workflows/.NET%20Core/badge.svg?branch=master)

# .NET Event Manager

A type safe event manager library for .NET.
Due the use of generic, this event manager doesn't suffer for boxing and unboxing of value types nor for casting errors of the consumers.
Additionaly, closures of delegates can be stored apart in order to reuse the delegate and reduce allocations.
Finally, it has support for weak-refence listeners.

The following example show some of the functions of the event manager:
```cs
public static class Player
{
	private static EventManager<IPlayerEvents> eventManager = new EventManager<IPlayerEvents>();

	public static void Main()
	{
		eventManager.Subscribe<PlayerHurtEvent>("Ouch", OnPlayerHurt);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(("Excalibur", "Mimic"), OnPlayerPickedUpItem2);

		eventManager.Raise(new PlayerPickedUpItemEvent("Excalibur"));

		eventManager.SubscribeOnce<PlayerPickedUpItemEvent>(OnPlayerPickedUpItemOnce);

		eventManager.Raise(new PlayerPickedUpItemEvent("Pencil"));

		eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));

		eventManager.Unsubscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem2);

		eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));
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
			eventManager.Raise(new PlayerHurtEvent());
		}
	}
	
	private static void OnPlayerPickedUpItemOnce(PlayerPickedUpItemEvent @event)
		=> Console.WriteLine($"This was registered to only execute once.");

	public interface IPlayerEvents { }

	public readonly struct PlayerHurtEvent : IPlayerEvents { }

	public readonly struct PlayerPickedUpItemEvent : IPlayerEvents
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
	public void Subscribe<TEvent>(Action<TEvent> callback);
	public void Subscribe<TEvent>(Action callback);
	public void Unsubscribe<TEvent>(Action<TEvent> callback);
	public void Unsubscribe<TEvent>(Action callback);
	
	/// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	
	/// Subscribes an action to run the next time the event `TEvent` is raised.
	public void SubscribeOnce<TEvent>(Action<TEvent> callback);
	public void SubscribeOnce<TEvent>(Action callback);
	public void UnsubscribeOnce<TEvent>(Action<TEvent> callback);
	public void UnsubscribeOnce<TEvent>(Action callback);
		
	/// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	
	/// Subscribes an action to run when the event `TEvent` is raised.
	/// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback);
	
	/// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	/// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	
	/// Subscribes an action to run the next time the event `TEvent` is raised.
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback);
		
	/// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	
	/// Raise the specified event.
	public void Raise<TEvent>(TEvent eventArgument);
	
	/// Dispose the underlying content of this event manager.
	public void Dispose();
	
	/// Forces the purge of removed delegates to avoid memory leaks.
	/// This method is only required to execute, if you unsubscribed too many listeners of a given `TEvent`
	/// but haven't executed `Raise<TEvent>` yet and want to release their references.
	public void Purge();
}
```

## Changelog

### v0.1.0

Initial Release

### v0.2.0

- Remove `TEventBase` generic parameter from `EventManager<TEventBase>`.
- Become `EventManager` thread-safe.
- Increase perfomance in `EventManager` subscribe and unsubscribe methods by replacing `Delegate.Combine` and `Delegate.Remove` with pooled arrays of delegates.
- Add target frameworks for .Net Standard 2.1 and .Net 5.
- Add the following APIs:
```cs
public sealed partial class EventManager : IDisposable
{
	/// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	
	/// Subscribes an action to run the next time the event `TEvent` is raised.
	public void SubscribeOnce<TEvent>(Action<TEvent> callback);
	public void SubscribeOnce<TEvent>(Action callback);
	public void UnsubscribeOnce<TEvent>(Action<TEvent> callback);
	public void UnsubscribeOnce<TEvent>(Action callback);
		
	/// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
	
	/// Subscribes an action to run when the event `TEvent` is raised.
	/// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback);
	
	/// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	/// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);	
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	
	/// Subscribes an action to run the next time the event `TEvent` is raised.
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback);
		
	/// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback);
	public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback);
}
```

## v0.2.1

- Apply nullable analysis on API.
- Fix some null reference error bugs.

## v0.3

- Fix exceptions when raising events.
- Fix memory leaks of weak subscribers.
- Reduce garbage produced when a subscribed delegate throws.
- Become auto-cleaning abortable and incremental.
- Add internal multithreading on `.Dispose()` method and auto-cleaning.
- Check for math overflow and underflow on debug builds.