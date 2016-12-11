using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LD37.Entities.Abstract;
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

		public Body CreateBody(Entity entity)
		{
			return BodyFactory.CreateBody(world, entity);
		}

		public Body CreateEdge(Vector2 start, Vector2 end, Units units, Entity entity)
		{
			if (units == Units.Pixels)
			{
				start = PhysicsConvert.ToMeters(start);
				end = PhysicsConvert.ToMeters(end);
			}

			Body body = BodyFactory.CreateEdge(world, start, end);
			body.UserData = entity;

			return body;
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

		public void AttachEdge(Body body, Vector2 start, Vector2 end, Units units)
		{
			if (units == Units.Pixels)
			{
				start = PhysicsConvert.ToMeters(start);
				end = PhysicsConvert.ToMeters(end);
			}

			FixtureFactory.AttachEdge(start, end, body);
		}
	}
}
