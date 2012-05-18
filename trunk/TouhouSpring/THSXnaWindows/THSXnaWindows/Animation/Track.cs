using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Animation
{
	abstract class Track
	{
		public abstract float Duration
		{
			get;
		}

		public float Time
		{
			get; private set;
		}

		public float TimeFactor
		{
			get; set;
		}

		public float CurrentValue
		{
			get; private set;
		}

		public bool IsPlaying
		{
			get; private set;
		}

		public bool Loop
		{
			get; set;
		}

		public event Action<float> Elapsed;

		protected Track()
		{
			Time = 0.0f;
			TimeFactor = 1.0f;
			IsPlaying = false;
			Loop = false;
		}

		public void Play()
		{
			PlayFrom(0.0f);
		}

		public void PlayFrom(float time)
		{
			if (IsPlaying)
			{
				throw new InvalidOperationException("The track is playing.");
			}
			else if (time < 0 || time > Duration)
			{
				throw new ArgumentOutOfRangeException("time");
			}

			Time = time;
			IsPlaying = true;
			CurrentValue = Evaluate(Time);
		}

		public void Stop()
		{
			if (!IsPlaying)
			{
				throw new InvalidOperationException("The track is not playing.");
			}

			IsPlaying = false;
		}

		public void Elapse(float deltaTime)
		{
			if (deltaTime < 0.0f)
			{
				throw new ArgumentOutOfRangeException("Delta time shall be non-negative.");
			}
			else if (!IsPlaying)
			{
				throw new InvalidOperationException("The track is not playing.");
			}

			Time += deltaTime * TimeFactor;
			if ((Time > Duration || Time < 0.0f) && Loop)
			{
				Time = Time % Duration;
			}
			else if (Time > Duration)
			{
				Time = Duration;
				Stop();
			}
			else if (Time < 0)
			{
				Time = 0;
				Stop();
			}
			CurrentValue = Evaluate(Time);

			if (Elapsed != null)
			{
				Elapsed(CurrentValue);
			}
		}

		public abstract float Evaluate(float time);
	}
}
