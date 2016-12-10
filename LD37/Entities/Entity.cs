using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	internal abstract class Entity : IDynamic, IRenderable
	{
		private Vector2 position;
		private Vector2 scale;

		private float rotation;

		public virtual Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		[JsonProperty]
		public virtual Vector2 LoadPosition
		{
			get { return position; }
			set { Position = value * Constants.TileSize + new Vector2(Constants.TileSize) / 2; }
		}

		public virtual Vector2 Scale
		{
			get { return scale; }
			set { scale = value; }
		}

		public virtual float Rotation
		{
			get { return rotation; }
			set { rotation = value; }
		}

		public virtual string EntityGroup => null;
		public virtual bool TileAttach => false;

		public virtual void Update(float dt)
		{
		}

		public abstract void Render(SpriteBatch sb);
	}
}
