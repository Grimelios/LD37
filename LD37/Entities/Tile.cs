﻿using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Tile : Entity
	{
		private static int flipTime;

		static Tile()
		{
			PropertyMap properties = Properties.Load("Miscellaneous.properties");
			flipTime = int.Parse(properties["Tile.Flip.Time"]);
		}

		private Sprite sprite;
		private Timer timer;

		private bool endGame;

		public Tile(ContentLoader contentLoader)
		{
			sprite = new Sprite(contentLoader, "Tile", OriginLocations.Center);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				sprite.Scale = value;

				if (AttachedEntity != null)
				{
					AttachedEntity.Scale = value;
				}

				if (ReversedEntity != null)
				{
					ReversedEntity.Scale = value;
				}
			}
		}

		public override string EntityGroup => "Tile";

		public Entity AttachedEntity { get; set; }
		public Entity ReversedEntity { get; set; }

		public void Flip(float initialDelay = 0, bool endGame = false)
		{
			this.endGame = endGame;

			timer = new Timer(initialDelay, BeginFlip);
		}

		private void BeginFlip()
		{
			timer = new Timer(flipTime, (progress) =>
			{
				Scale = new Vector2(1 - progress * progress, 1);
			}, ReverseFlip);
		}

		private void ReverseFlip()
		{
			if (endGame)
			{
				timer = null;
				Scale = Vector2.Zero;

				return;
			}

			AttachedEntity = ReversedEntity;
			ReversedEntity = null;

			timer = new Timer(flipTime, (progress) =>
			{
				Scale = new Vector2(-progress * (progress - 2), 1);
			}, () =>
			{
				Scale = Vector2.One;
				timer = null;
			}, timer.Elapsed);
		}

		public override void Update(float dt)
		{
			timer?.Update(dt);
			AttachedEntity?.Update(dt);
			ReversedEntity?.Update(dt);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
			AttachedEntity?.Render(sb);
		}
	}
}
