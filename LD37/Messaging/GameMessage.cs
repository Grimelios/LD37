namespace LD37.Messaging
{
	internal enum MessageTypes
	{
		Mouse,
		Keyboard,
		Gamepad,
		LevelAdvance
	}

	internal class GameMessage
	{
		public GameMessage(MessageTypes type)
		{
			Type = type;
		}

		public MessageTypes Type { get; }
	}
}
