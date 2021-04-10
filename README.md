![.NET Core](https://github.com/Enderlook/Net-Event-Manager/workflows/.NET%20Core/badge.svg?branch=master)

# .NET Event Manager

A type safe event manager library for .NET.
Due the use of generic, this event manager doesn't suffer for boxing and unboxing of value types nor for casting errors of the consumers.
Additionaly, closures of delegates can be stored apart in order to reuse the delegate and reduce allocations.

The following example show all the functions of the event manager:
```cs
public static class Player
{
	private static EventManager<IPlayerEvents> eventManager = new EventManager<IPlayerEvents>();

	public static void Main()
	{
		eventManager.Subscribe<PlayerHurtEvent>("Ouch", OnPlayerHurt);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(("Excalibur, "Mimic"), OnPlayerPickedUpItem2);

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
/// `TEventBase` is base type of all events. Useful to determine a common ground. (You can use `object` if you don't need it).
public sealed class EventManager<TEventBase> : IDisposable
{
	/// Subscribes an action to run when the event `TEvent` is raised.
	public void Subscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
	public void Subscribe<TEvent>(Action callback) where TEvent : TEventBase
	public void Unsubscribe<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
	public void Unsubscribe<TEvent>(Action callback) where TEvent : TEventBase
	
	/// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
	public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
	public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
	
	/// Subscribes an action to run the next time the event `TEvent` is raised.
	public void SubscribeOnce<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
	public void SubscribeOnce<TEvent>(Action callback) where TEvent : TEventBase
	public void UnsubscribeOnce<TEvent>(Action<TEvent> callback) where TEvent : TEventBase
	public void UnsubscribeOnce<TEvent>(Action callback) where TEvent : TEventBase
		
	/// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
	public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback) where TEvent : TEventBase
	public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback) where TEvent : TEventBase
	
	/// Raise the specified event.
	public void Raise<TEvent>(TEvent eventArgument) where TEvent : TEventBase
	
	/// Dispose the underlying content of this event manager.
	public void Dispose()
	
	/// Forces the purge of removed delegates to avoid memory leaks.
	/// This method is only required to execute, if you unsubscribed too many listeners of a given `TEvent`
	/// but haven't executed `Raise<TEvent>` yet and want to release their references.
	public void Purge()
}
```