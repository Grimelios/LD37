using LD37.Input;

namespace LD37.Messaging.Input
{
	internal class MouseMessage : GameMessage
	{
		public MouseMessage(MouseData data) : base(MessageTypes.Mouse)
		{
			Data = data;
		}

		public MouseData Data { get; }
	}
}
