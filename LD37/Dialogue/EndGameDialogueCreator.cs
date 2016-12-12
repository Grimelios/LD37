using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Entities.Organization;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Ninject;

namespace LD37.Dialogue
{
	internal class EndGameDialogueCreator : IDynamic
	{
		private const int DialogueDelay1 = 3500;
		private const int DialogueDelay2 = 4500;
		private const int DialogueOffset1 = 100;
		private const int DialogueOffset2 = 130;

		private Timer timer;
		private List<Entity> dialogueLines;
		private StandardKernel kernel;

		public EndGameDialogueCreator(Scene scene, StandardKernel kernel)
		{
			this.kernel = kernel;

			dialogueLines = scene.LayerMap["Primary"].EntityMap["Dialogue"];
			timer = new Timer(DialogueDelay1, CreatePrimaryDialogue);
		}

		private void CreatePrimaryDialogue()
		{
			DialogueLine dialogueLine = kernel.Get<DialogueLine>();
			dialogueLine.FontFilename = "Primary";
			dialogueLine.Value = "Congratulations! You have finished all levels. You are a genius problem solver.";
			dialogueLine.Position = new Vector2(Constants.ScreenWidth / 2, DialogueOffset1);
			dialogueLines.Add(dialogueLine);

			timer = new Timer(DialogueDelay2, CreateSecondaryDialogue);
		}

		private void CreateSecondaryDialogue()
		{
			DialogueLine dialogueLine = kernel.Get<DialogueLine>();
			dialogueLine.FontFilename = "Secondary";
			dialogueLine.Value = "GG EZ";
			dialogueLine.Position = new Vector2(Constants.ScreenWidth / 2, DialogueOffset2);
			dialogueLines.Add(dialogueLine);

			timer = null;
		}

		public void Update(float dt)
		{
			timer?.Update(dt);
		}
	}
}
