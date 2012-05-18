using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
	public struct Particle
	{
		public float m_life;
		public float m_age;

		public Vector3 m_position;
		public Vector3 m_velocity;

		public Vector2 m_size;
		public float m_rotation;

		public Color m_color;
	}
}
