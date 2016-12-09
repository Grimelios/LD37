using LD37.Messaging;

namespace LD37.Interfaces
{
	internal interface IMessageReceiver
	{
		void Receive(GameMessage message);
	}
}
