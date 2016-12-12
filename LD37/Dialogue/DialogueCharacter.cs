using LD37.Core;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Dialogue
{
	internal class DialogueCharacter : IDynamic, IRenderable
	{
		private const int RevealTime = 100;

		private SpriteText spriteText;
		private Timer timer;

		public DialogueCharacter(SpriteFont font, char character, Vector2 position, Color color)
		{
			spriteText = new SpriteText(font, character.ToString(), position, color);

			timer = new Timer(RevealTime, (progress) =>
			{
				spriteText.Scale = progress;
			}, () =>
			{
				spriteText.Scale = 1;
				timer = null;
			});
		}

		public void Update(float dt)
		{
			timer?.Update(dt);
		}

		public void Render(SpriteBatch sb)
		{
			spriteText.Render(sb);
		}
	}
}
