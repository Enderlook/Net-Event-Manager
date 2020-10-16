![.NET Core](https://github.com/Enderlook/Net-Event-Manager/workflows/.NET%20Core/badge.svg?branch=master)

# .NET Event Manager

A type safe event manager library for .NET.
Due the use of generic, this event manager doesn't suffer for boxing and unboxing of value types nor for casting errors of the consumers.

The following example show all the functions of the event manager:
```cs
public static class Player
{
	private static EventManager<IPlayerEvents> eventManager = new EventManager<IPlayerEvents>();

	public static void Main()
	{
		eventManager.Subscribe<PlayerHurtEvent>(OnPlayerHurt);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem);

		eventManager.Subscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem2);

		eventManager.Raise(new PlayerPickedUpItemEvent("Excalibur"));

		eventManager.Raise(new PlayerPickedUpItemEvent("Pencil"));

		eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));

		eventManager.Unsubscribe<PlayerPickedUpItemEvent>(OnPlayerPickedUpItem2);

		eventManager.Raise(new PlayerPickedUpItemEvent("Mimic"));
	}

	private static void OnPlayerHurt() => Console.WriteLine("Ouch!");

	private static void OnPlayerPickedUpItem(PlayerPickedUpItemEvent @event) => Console.WriteLine($"Picked {@event.Item}");

	private static void OnPlayerPickedUpItem2(PlayerPickedUpItemEvent @event)
	{
		if (@event.Item == "Exalibur")
			Console.WriteLine("Player picked up legendary sword!");
		else if (@event.Item == "Mimic")
		{
			Console.WriteLine("Oh no! You picked up a mimic!");
			eventManager.Raise(new PlayerHurtEvent());
		}
	}

	public interface IPlayerEvents { }

	public readonly struct PlayerHurtEvent : IPlayerEvents { }

	public readonly struct PlayerPickedUpItemEvent : IPlayerEvents
	{
		public readonly string Item;

		public PlayerPickedUpItemEvent(string item) => Item = item;
	}
}
```