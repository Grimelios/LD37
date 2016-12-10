using System.Collections.Generic;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class Laser : Entity
	{
		private const int RayCastRange = 40;

		private List<Vector2> points;
		private PhysicsHelper physicsHelper;
		private PrimitiveDrawer primitiveDrawer;

		public Laser(PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer)
		{
			this.physicsHelper = physicsHelper;
			this.primitiveDrawer = primitiveDrawer;

			points = new List<Vector2>();
		}

		public Color Color { get; set; }

		public void RecomputePoints(Vector2 source, float angle)
		{
			points.Clear();
			points.Add(source);

			Vector2 currentSource = source;
			Mirror mirror;

			float currentAngle = angle;

			do
			{
				RayCastResults results = physicsHelper.RayCast(currentSource, currentAngle, RayCastRange);
				mirror = results.Entity as Mirror;

				if (mirror != null)
				{
					Vector2 position = results.Position;

					points.Add(position);
					currentSource = results.Position;
					currentAngle = mirror.ComputeReflectionAngle(angle);
				}
			} while (mirror != null);
		}

		public override void Render(SpriteBatch sb)
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				primitiveDrawer.DrawLine(sb, points[i], points[i + 1], Color);
			}
		}
	}
}
