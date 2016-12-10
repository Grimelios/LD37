using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Physics
{
	public class PhysicsDebugDrawer : IRenderable
	{
		private PrimitiveDrawer primitiveDrawer;
		private World world;

		public PhysicsDebugDrawer(PrimitiveDrawer primitiveDrawer, World world)
		{
			this.primitiveDrawer = primitiveDrawer;
			this.world = world;
		}

		public void Render(SpriteBatch sb)
		{
			foreach (Body body in world.BodyList)
			{
				Vector2 bodyPosition = PhysicsConvert.ToPixels(body.Position);

				float bodyRotation = body.Rotation;

				foreach (Fixture fixture in body.FixtureList)
				{
					Shape shape = fixture.Shape;

					switch (shape.ShapeType)
					{
						case ShapeType.Edge:
							RenderEdge(sb, (EdgeShape)shape, bodyPosition);
							break;

						case ShapeType.Polygon:
							RenderPolygon(sb, (PolygonShape)shape, bodyPosition, bodyRotation);
							break;
					}
				}
			}
		}

		private void RenderEdge(SpriteBatch sb, EdgeShape shape, Vector2 bodyPosition)
		{
			Vector2 start = PhysicsConvert.ToPixels(bodyPosition + shape.Vertex1);
			Vector2 end = PhysicsConvert.ToPixels(bodyPosition + shape.Vertex2);

			primitiveDrawer.DrawLine(sb, start, end, Color.Orange);
		}

		private void RenderPolygon(SpriteBatch sb, PolygonShape shape, Vector2 bodyPosition, float bodyRotation)
		{
			Vertices vertices = shape.Vertices;
			Matrix rotationMatrix = Matrix.CreateRotationZ(bodyRotation);

			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vertex1 = vertices[i];
				Vector2 vertex2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];
				Vector2 start = bodyPosition + Vector2.Transform(PhysicsConvert.ToPixels(vertex1), rotationMatrix);
				Vector2 end = bodyPosition + Vector2.Transform(PhysicsConvert.ToPixels(vertex2), rotationMatrix);

				primitiveDrawer.DrawLine(sb, start, end, Color.Orange);
			}
		}
	}
}
