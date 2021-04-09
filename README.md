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