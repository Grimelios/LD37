using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class Laser : Entity
	{
		private List<Vector2> points;
		private PrimitiveDrawer primitiveDrawer;

		public Laser(PrimitiveDrawer primitiveDrawer)
		{
			this.primitiveDrawer = primitiveDrawer;
		}

		public Color Color { get; set; }

		public override void Render(SpriteBatch sb)
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				primitiveDrawer.DrawLine(sb, points[i], points[i + 1], Color);
			}
		}
	}
}
