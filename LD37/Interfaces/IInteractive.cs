using Microsoft.Xna.Framework;

namespace LD37.Interfaces
{
	internal interface IInteractive
	{
		Rectangle InteractionBox { get; }

		void InteractionResponse();
	}
}
