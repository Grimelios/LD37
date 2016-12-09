using LD37.Input;

namespace LD37.Messaging.Input
{
	internal class KeyboardMessage : GameMessage
	{
		public KeyboardMessage(KeyboardData data) : base(MessageTypes.Keyboard)
		{
			Data = data;
		}

		public KeyboardData Data { get; }
	}
}
