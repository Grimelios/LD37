using System.Collections.Generic;
using LD37.Interfaces;

namespace LD37.Messaging
{
	internal class MessageSystem
	{
		public MessageSystem()
		{
			ReceiverLists = new List<IMessageReceiver>[GameFunctions.EnumCount(typeof(MessageTypes))];
		}

		public List<IMessageReceiver>[] ReceiverLists { get; }

		public void Subscribe(MessageTypes messageType, IMessageReceiver receiver)
		{
			VerifyList(messageType);
			ReceiverLists[(int)messageType].Add(receiver);
		}

		public void Unsubscribe(int messageType, IMessageReceiver receiver)
		{
			ReceiverLists[messageType].Remove(receiver);
		}

		public void Send(GameMessage message)
		{
			MessageTypes messageType = message.Type;

			VerifyList(messageType);
			ReceiverLists[(int)messageType].ForEach(r => r.Receive(message));
		}

		private void VerifyList(MessageTypes messageType)
		{
			int index = (int)messageType;

			if (ReceiverLists[index] == null)
			{
				ReceiverLists[index] = new List<IMessageReceiver>();
			}
		}
	}
}
