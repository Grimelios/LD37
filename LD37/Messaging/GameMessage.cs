namespace LD37.Messaging
{
	internal enum MessageTypes
	{
		Mouse,
		Keyboard,
		Gamepad
	}

	internal abstract class GameMessage
	{
		protected GameMessage(MessageTypes type)
		{
			Type = type;
		}

		public MessageTypes Type { get; }
	}
}
