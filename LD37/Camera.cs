using LD37.Interfaces;
using Microsoft.Xna.Framework;

namespace LD37
{
	public class Camera : IDynamic
	{
		public Camera()
		{
			Zoom = 1;
			Transform = Matrix.Identity;
			InverseTransform = Matrix.Identity;
		}

		public Vector2 Origin { get; set; }
		public Vector2 Position { get; set; }

		public float Zoom { get; set; }
		public float Rotation { get; set; }

		public Matrix Transform { get; private set; }
		public Matrix InverseTransform { get; private set; }

		public void Update(float dt)
		{
			Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				Matrix.CreateScale(Zoom) *
				Matrix.CreateRotationZ(Rotation) *
				Matrix.CreateTranslation(new Vector3(Origin, 0));
			InverseTransform = Matrix.Invert(Transform);
		}
	}
}
