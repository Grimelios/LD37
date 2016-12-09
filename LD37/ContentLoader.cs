using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD37
{
	public class ContentLoader
	{
		private ContentManager content;

		public ContentLoader(ContentManager content)
		{
			this.content = content;
		}

		public Texture2D LoadTexture(string filename)
		{
			return content.Load<Texture2D>("Textures/" + filename);
		}

		public SpriteFont LoadFont(string filename)
		{
			return content.Load<SpriteFont>("Fonts/" + filename);
		}
	}
}
