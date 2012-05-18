using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Animation
{
	class TrackUpdater : Services.GameService
	{
		private List<Track> m_tracks = new List<Track>();

		public void Register(Track track)
		{
			if (track == null)
			{
				throw new ArgumentNullException("track");
			}
			else if (m_tracks.Contains(track))
			{
				throw new ArgumentException("Track has already been registered.");
			}

			m_tracks.Add(track);
		}

		public void Unregister(Track track)
		{
			if (track == null)
			{
				throw new ArgumentNullException("track");
			}
			else if (!m_tracks.Remove(track))
			{
				throw new ArgumentException("Track is not registered.");
			}
		}

		public override void Update(float deltaTime)
		{
			m_tracks.ForEach(track =>
			{
				if (track.IsPlaying)
				{
					track.Elapse(deltaTime);
				}
			});
		}
	}
}
