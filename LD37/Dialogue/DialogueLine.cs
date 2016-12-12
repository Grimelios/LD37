using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Dialogue
{
	internal class DialogueLine : Entity
	{
		private const int CharacterDelay = 50;

		private SpriteFont font;
		private Timer timer;
		private Vector2 basePosition;
		private ContentLoader contentLoader;
		private List<DialogueCharacter> characters;

		private string fullValue;
		private string revealedSoFar;

		private int characterIndex;

		public DialogueLine(ContentLoader contentLoader)
		{
			this.contentLoader = contentLoader;

			timer = new Timer(CharacterDelay, AdvanceCharacter);
			characters = new List<DialogueCharacter>();
		}

		public override Vector2 Position
		{
			set
			{
				basePosition = value - font.MeasureString(fullValue) / 2;

				base.Position = value;
			}
		}

		public string FontFilename
		{
			set
			{
				font = contentLoader.LoadFont(value);
				contentLoader = null;
			}
		}

		public string Value
		{
			set
			{
				fullValue = value;
				revealedSoFar = "";
			}
		}

		private void AdvanceCharacter()
		{
			Vector2 characterPosition = basePosition + new Vector2(font.MeasureString(revealedSoFar).X, 0);
			characterPosition.X = (int)characterPosition.X;
			characterPosition.Y = (int)characterPosition.Y;

			DialogueCharacter character = new DialogueCharacter(font, fullValue[characterIndex], characterPosition, Color.White);
			characters.Add(character);

			if (characterIndex == fullValue.Length - 1)
			{
				timer = null;
			}
			else
			{
				characterIndex++;
				revealedSoFar = fullValue.Substring(0, characterIndex);
			}
		}

		public override void Update(float dt)
		{
			timer?.Update(dt);
			characters.ForEach(c => c.Update(dt));
		}

		public override void Render(SpriteBatch sb)
		{
			characters.ForEach(c => c.Render(sb));
		}
	}
}
