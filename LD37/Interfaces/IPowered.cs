using Microsoft.Xna.Framework;

namespace LD37.Interfaces
{
	internal interface IPowered
	{
		int PowerID { get; set; }
		bool Powered { get; set; }

		Vector2 WirePosition { get; }
	}
}
