using Microsoft.Xna.Framework;

namespace LD37.Interfaces
{
	internal interface IInteractive
	{
		Rectangle BoundingBox { get; }

		void InteractionResponse();
	}
}
