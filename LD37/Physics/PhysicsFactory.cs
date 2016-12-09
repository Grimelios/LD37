using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LD37.Entities;
using Microsoft.Xna.Framework;

namespace LD37.Physics
{
	internal enum Units
	{
		Pixels,
		Meters
	}

	internal class PhysicsFactory
	{
		private World world;

		public PhysicsFactory(World world)
		{
			this.world = world;
		}

		public Body CreateRectangle(float width, float height, Units units, BodyType bodyType, Entity entity)
		{
			return CreateRectangle(width, height, Vector2.Zero, units, bodyType, entity);
		}

		public Body CreateRectangle(float width, float height, Vector2 position, Units units, BodyType bodyType, Entity entity)
		{
			if (units == Units.Pixels)
			{
				width = PhysicsConvert.ToMeters(width);
				height = PhysicsConvert.ToMeters(height);
				position = PhysicsConvert.ToMeters(position);
			}

			Body body = BodyFactory.CreateRectangle(world, width, height, 1, position, entity);
			body.BodyType = bodyType;

			return body;
		}
	}
}
