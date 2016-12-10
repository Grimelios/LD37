using System;
using LD37.Interfaces;

namespace LD37.Core
{
	public class Timer : IDynamic
	{
		private Action<float> tick;
		private Action trigger;

		private bool repeating;

		public Timer(float duration, Action trigger, bool repeating, float initialElapsed = 0) :
			this(duration, null, trigger, repeating)
		{
		}

		public Timer(float duration, Action<float> tick, Action trigger, bool repeating, float initialElapsed = 0)
		{
			this.tick = tick;
			this.trigger = trigger;
			this.repeating = repeating;

			Duration = duration;
			Elapsed = initialElapsed;
		}

		public bool Paused { get; set; }
		public bool Completed { get; private set; }

		public float Elapsed { get; set; }
		public float Duration { get; set; }

		public void Update(float dt)
		{
			if (Paused)
			{
				return;
			}

			Elapsed += dt * 1000;

			if (Elapsed >= Duration)
			{
				Elapsed -= Duration;
				trigger();

				if (!Completed)
				{
					Completed = true;

					return;
				}
			}

			tick?.Invoke(Elapsed / Duration);
		}
	}
}
