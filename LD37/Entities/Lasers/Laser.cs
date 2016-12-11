using System.Collections.Generic;
using LD37.Entities.Abstract;
using LD37.Interfaces;
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
		private LaserReceiver activatedReceiver;

		public Laser(PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer)
		{
			this.physicsHelper = physicsHelper;
			this.primitiveDrawer = primitiveDrawer;

			points = new List<Vector2>();
		}

		public Color Color { get; set; }

		public void Unpower()
		{
			if (activatedReceiver != null)
			{
				activatedReceiver.Powered = false;
				activatedReceiver = null;
			}
		}

		public void Recast(Vector2 source, float angle)
		{
			points.Clear();
			points.Add(source);

			Vector2 currentSource = PhysicsConvert.ToMeters(source);
			Mirror mirror;
			LaserReceiver receiver = null;

			float currentAngle = angle;

			do
			{
				RayCastResults results = physicsHelper.RayCast(currentSource, currentAngle, RayCastRange);
				points.Add(PhysicsConvert.ToPixels(results.Position));
				mirror = results.Entity as Mirror;

				if (mirror != null)
				{
					currentSource = PhysicsConvert.ToMeters(results.Position);
					currentAngle = mirror.ComputeReflectionAngle(angle);
				}
				else
				{
					receiver = results.Entity as LaserReceiver;
				}
			} while (mirror != null);

			if (receiver != activatedReceiver)
			{
				if (receiver == null)
				{
					activatedReceiver.Powered = false;
					activatedReceiver = null;
				}
				else
				{
					activatedReceiver = receiver;
					activatedReceiver.Powered = true;
				}
			}
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
