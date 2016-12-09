using Microsoft.Xna.Framework;

namespace LD37.Input
{
	internal class GamepadData
	{
		public GamepadData(ClickStates a, ClickStates b, ClickStates x, ClickStates y, ClickStates start, ClickStates back,
			ClickStates leftBumper, ClickStates rightBumper, ClickStates l3, ClickStates r3, ClickStates dPadLeft, ClickStates dPadRight,
			ClickStates dPadUp, ClickStates dPadDown, Vector2 leftStick, Vector2 rightStick, float leftTrigger, float rightTrigger)
		{
			A = a;
			B = b;
			X = x;
			Y = y;
			Start = start;
			Back = back;
			LeftBumper = leftBumper;
			RightBumper = rightBumper;
			L3 = l3;
			R3 = r3;
			DPadLeft = dPadLeft;
			DPadRight = dPadRight;
			DPadUp = dPadUp;
			DPadDown = dPadDown;
			LeftStick = leftStick;
			RightStick = rightStick;
			LeftTrigger = leftTrigger;
			RightTrigger = rightTrigger;
		}

		public ClickStates A { get; }
		public ClickStates B { get; }
		public ClickStates X { get; }
		public ClickStates Y { get; }
		public ClickStates Start { get; }
		public ClickStates Back { get; }
		public ClickStates LeftBumper { get; }
		public ClickStates RightBumper { get; }
		public ClickStates L3 { get; }
		public ClickStates R3 { get; }
		public ClickStates DPadLeft { get; }
		public ClickStates DPadRight { get; }
		public ClickStates DPadUp { get; }
		public ClickStates DPadDown { get; }

		public Vector2 LeftStick { get; }
		public Vector2 RightStick { get; }

		public float LeftTrigger { get; }
		public float RightTrigger { get; }
	}
}
