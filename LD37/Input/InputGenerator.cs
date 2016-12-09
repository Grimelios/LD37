using System.Collections.Generic;
using System.Linq;
using LD37.Messaging;
using LD37.Messaging.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD37.Input
{
	internal class InputGenerator
	{
		private Camera camera;
		private MessageSystem messageSystem;
		private KeyboardState oldKS;
		private KeyboardState newKS;
		private MouseState oldMS;
		private MouseState newMS;
		private GamePadState oldGPS;
		private GamePadState newGPS;

		public InputGenerator(Camera camera, MessageSystem messageSystem)
		{
			this.camera = camera;
			this.messageSystem = messageSystem;
		}

		public void GenerateInputMessages()
		{
			GenerateKeyboardMessage();
			GenerateMouseMessage();
			GenerateGamepadMessage();
		}

		private void GenerateKeyboardMessage()
		{
			oldKS = newKS;
			newKS = Keyboard.GetState();

			List<Keys> oldKeysDown = oldKS.GetPressedKeys().ToList();
			List<Keys> newKeysDown = newKS.GetPressedKeys().ToList();
			List<Keys> keysPressedThisFrame = newKeysDown.Except(oldKeysDown).ToList();
			List<Keys> keysReleasedThisFrame = oldKeysDown.Except(newKeysDown).ToList();

			messageSystem.Send(new KeyboardMessage(new KeyboardData(newKeysDown, keysPressedThisFrame, keysReleasedThisFrame)));
		}

		private void GenerateMouseMessage()
		{
			oldMS = newMS;
			newMS = Mouse.GetState();

			Vector2 screenPosition = new Vector2(newMS.X, newMS.Y);
			Vector2 worldPosition = Vector2.Transform(screenPosition, camera.InverseTransform);

			ClickStates leftClickState = GetClickState(oldMS.LeftButton, newMS.LeftButton);
			ClickStates rightClickState = GetClickState(oldMS.RightButton, newMS.RightButton);

			messageSystem.Send(new MouseMessage(new MouseData(screenPosition, worldPosition, leftClickState, rightClickState)));
		}

		private void GenerateGamepadMessage()
		{
			oldGPS = newGPS;
			newGPS = GamePad.GetState(PlayerIndex.One);

			GamePadButtons oldButtons = oldGPS.Buttons;
			GamePadButtons newButtons = newGPS.Buttons;

			ClickStates a = GetClickState(oldButtons.A, newButtons.A);
			ClickStates b = GetClickState(oldButtons.B, newButtons.B);
			ClickStates x = GetClickState(oldButtons.X, newButtons.X);
			ClickStates y = GetClickState(oldButtons.Y, newButtons.Y);
			ClickStates start = GetClickState(oldButtons.Start, newButtons.Start);
			ClickStates back = GetClickState(oldButtons.Back, newButtons.Back);
			ClickStates leftBumper = GetClickState(oldButtons.LeftShoulder, newButtons.LeftShoulder);
			ClickStates rightBumper = GetClickState(oldButtons.RightShoulder, newButtons.RightShoulder);
			ClickStates l3 = GetClickState(oldButtons.LeftStick, newButtons.LeftStick);
			ClickStates r3 = GetClickState(oldButtons.RightStick, newButtons.RightStick);

			GamePadThumbSticks thumbsticks = newGPS.ThumbSticks;

			Vector2 leftStick = thumbsticks.Left;
			Vector2 rightStick = thumbsticks.Right;

			GamePadTriggers triggers = newGPS.Triggers;

			float leftTrigger = triggers.Left;
			float rightTrigger = triggers.Right;

			GamePadDPad oldDPad = oldGPS.DPad;
			GamePadDPad newDPad = newGPS.DPad;

			ClickStates dPadLeft = GetClickState(oldDPad.Left, newDPad.Left);
			ClickStates dPadRight = GetClickState(oldDPad.Right, newDPad.Right);
			ClickStates dPadUp = GetClickState(oldDPad.Up, newDPad.Up);
			ClickStates dPadDown = GetClickState(oldDPad.Down, newDPad.Down);

			messageSystem.Send(new GamepadMessage(new GamepadData(a, b, x, y, start, back, leftBumper, rightBumper, l3, r3, dPadLeft,
				dPadRight, dPadDown, dPadUp, leftStick, rightStick, leftTrigger, rightTrigger)));
		}

		private ClickStates GetClickState(ButtonState oldState, ButtonState newState)
		{
			if (oldState == newState)
			{
				return newState == ButtonState.Pressed ? ClickStates.Held : ClickStates.Released;
			}

			return newState == ButtonState.Pressed ? ClickStates.PressedThisFrame : ClickStates.ReleasedThisFrame;
		}
	}
}
