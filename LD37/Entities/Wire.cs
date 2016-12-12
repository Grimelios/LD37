using System.Collections.Generic;
using LD37.Entities.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	internal class Wire : Entity
	{
		private PrimitiveDrawer primitiveDrawer;
		private Color color;

		private bool powered;

		public Wire(PrimitiveDrawer primitiveDrawer)
		{
			this.primitiveDrawer = primitiveDrawer;

			Points = new List<Vector2>();
		}

		[JsonProperty]
		public List<Vector2> Points { get; set; }

		[JsonProperty]
		public bool Powered
		{
			get { return powered; }
			set
			{
				powered = value;
				color = powered ? Color.DarkOrange : Color.Gray;
			}
		}

		[JsonProperty]
		public int TargetID { get; set; }

		public override string EntityGroup => "Wire";

		public override void Render(SpriteBatch sb)
		{
			for (int i = 0; i < Points.Count - 1; i++)
			{
				primitiveDrawer.DrawLine(sb, Points[i], Points[i + 1], color);
			}
		}
	}
}
