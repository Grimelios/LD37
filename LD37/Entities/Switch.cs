using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Switch : AbstractPowerSource, IInteractive
	{
		private static int interactionWidth;
		private static int interactionHeight;
		private static int toggleTime;

		static Switch()
		{
			PropertyMap properties = Properties.Load("Switch.properties");

			interactionWidth = int.Parse(properties["Interaction.Width"]);
			interactionHeight = int.Parse(properties["Interaction.Height"]);
			toggleTime = int.Parse(properties["Toggle.Time"]);
		}

		private Sprite mainSprite;
		private Sprite leverSprite;
		private Vector2 poweredLeverOffset;
		private Vector2 unpoweredLeverOffset;
		private Timer timer;

		public Switch(ContentLoader contentLoader, InteractionSystem interactionSystem)
		{
			Texture2D texture = contentLoader.LoadTexture("Switch");
			Rectangle mainSourceRect = new Rectangle(0, 0, 32, 32);
			Rectangle leverSourceRect = new Rectangle(32, 0, 4, 4);

			mainSprite = new Sprite(texture, mainSourceRect, OriginLocations.Center);
			leverSprite = new Sprite(texture, leverSourceRect, OriginLocations.Center);
			poweredLeverOffset = new Vector2(0, -8);
			unpoweredLeverOffset = new Vector2(0, 4);
			InteractionBox = new Rectangle(0, 0, interactionWidth, interactionHeight);

			interactionSystem.Items.Add(this);
		}

		public override Vector2 Position
		{
			set
			{
				mainSprite.Position = value;
				leverSprite.Position = value + (Powered ? poweredLeverOffset : unpoweredLeverOffset);

				Rectangle box = InteractionBox;
				box.X = (int)value.X - box.Width / 2;
				box.Y = (int)value.Y - box.Height / 2;
				InteractionBox = box;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				mainSprite.Scale = value;
				leverSprite.Scale = value;
			}
		}

		[JsonIgnore]
		public Rectangle InteractionBox { get; private set; }

		public override bool Powered
		{
			set
			{
				base.Powered = value;

				leverSprite.Position = Position + (Powered ? poweredLeverOffset : unpoweredLeverOffset);
			}
		}

		public void InteractionResponse()
		{
			Vector2 leverStart = Position + (Powered ? poweredLeverOffset : unpoweredLeverOffset);
			Vector2 leverEnd = Position + (Powered ? unpoweredLeverOffset : poweredLeverOffset);

			timer = new Timer(toggleTime, (progress) =>
			{
				leverSprite.Position = Vector2.Lerp(leverStart, leverEnd, progress);
			}, () =>
			{
				leverSprite.Position = leverEnd;
				timer = null;
			});

			Powered = !Powered;
		}

		public override void Update(float dt)
		{
			timer?.Update(dt);
		}

		public override void Render(SpriteBatch sb)
		{
			mainSprite.Render(sb);
			leverSprite.Render(sb);
		}
	}
}
