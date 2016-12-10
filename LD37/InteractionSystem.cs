using System.Collections.Generic;
using LD37.Interfaces;
using Microsoft.Xna.Framework;

namespace LD37
{
	internal class InteractionSystem
	{
		public InteractionSystem()
		{
			Items = new List<IInteractive>();
		}

		public List<IInteractive> Items { get; }

		public void CheckInteraction(Rectangle playerRect)
		{
			foreach (IInteractive item in Items)
			{
				if (playerRect.Intersects(item.InteractionBox))
				{
					item.InteractionResponse();

					return;
				}
			}
		}
	}
}
