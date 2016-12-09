using LD37.Input;

namespace LD37.Messaging.Input
{
	internal class GamepadMessage : GameMessage
	{
		public GamepadMessage(GamepadData data) : base(MessageTypes.Gamepad)
		{
			Data = data;
		}

		public GamepadData Data { get; }
	}
}
