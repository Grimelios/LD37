using System.Collections.Generic;
using LD37.Core;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Switch : Entity, IInteractive, IPowerSource
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

		private bool powered;

		private IPowered[] powerTargets;

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
				leverSprite.Position = value + (powered ? poweredLeverOffset : unpoweredLeverOffset);

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

		public Rectangle InteractionBox { get; private set; }

		[JsonProperty]
		public bool Powered
		{
			set
			{
				powered = value;

				if (powerTargets != null)
				{
					foreach (IPowered target in powerTargets)
					{
						target.Powered = value;
					}
				}

				leverSprite.Position = Position + (powered ? poweredLeverOffset : unpoweredLeverOffset);
			}
		}

		[JsonProperty]
		public int[] TargetIDs { get; set; }

		public IPowered[] PowerTargets
		{
			set
			{
				powerTargets = value;

				foreach (IPowered target in powerTargets)
				{
					target.Powered = powered;
				}
			}
		}

		public void InteractionResponse()
		{
			Vector2 leverStart = Position + (powered ? poweredLeverOffset : unpoweredLeverOffset);
			Vector2 leverEnd = Position + (powered ? unpoweredLeverOffset : poweredLeverOffset);

			timer = new Timer(toggleTime, (progress) =>
			{
				leverSprite.Position = Vector2.Lerp(leverStart, leverEnd, progress);
			}, () =>
			{
				leverSprite.Position = leverEnd;
				timer = null;
			});

			Powered = !powered;
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
