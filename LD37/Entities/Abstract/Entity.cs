using System;
using LD37.Interfaces;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities.Abstract
{
	internal abstract class Entity : IDynamic, IRenderable, IDisposable
	{
		private Vector2 position;
		private Vector2 scale;

		private float rotation;

		[JsonIgnore]
		public virtual Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}
		
		public virtual Vector2 LoadPosition
		{
			get
			{
				Point tileCoordinates = TileConvert.ToTile(position);

				return new Vector2(tileCoordinates.X, tileCoordinates.Y);
			}
			set { Position = TileConvert.ToPixels(value); }
		}
		
		[JsonIgnore]
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

		[JsonIgnore]
		public virtual string EntityGroup => null;

		public virtual void Dispose()
		{
		}

		public virtual void Update(float dt)
		{
		}

		public abstract void Render(SpriteBatch sb);
	}
}
