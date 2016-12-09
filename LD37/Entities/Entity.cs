using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	internal abstract class Entity : IDynamic, IRenderable
	{
		private Vector2 position;

		public virtual Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public virtual Vector2 LoadPosition
		{
			get { return position; }
			set { Position = value; }
		}

		public virtual void Update(float dt)
		{
		}

		public abstract void Render(SpriteBatch sb);
	}
}
