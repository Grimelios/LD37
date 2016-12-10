using FarseerPhysics.Dynamics;
using LD37.Entities;
using Microsoft.Xna.Framework;

namespace LD37.Physics
{
	internal class PhysicsHelper
	{
		private World world;

		public PhysicsHelper(World world)
		{
			this.world = world;
		}

		public RayCastResults RayCast(Vector2 source, float angle, float range)
		{
			Fixture closestFixture = null;
			Vector2 direction = GameFunctions.ComputeDirection(angle);
			Vector2 closestPoint = Vector2.Zero;

			world.RayCast((fixture, point, normal, fraction) =>
			{
				closestFixture = fixture;
				closestPoint = point;

				return fraction;
			}, source, source + direction * range);

			return new RayCastResults(closestPoint, (Entity)closestFixture.Body.UserData);
		}
	}
}
