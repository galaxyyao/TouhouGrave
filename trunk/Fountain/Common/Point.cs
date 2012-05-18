using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public struct Point
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Point(float x, float y) : this()
		{
			X = x;
			Y = y;
		}

		public bool IntersectsWith(Rectangle rect)
		{
			return rect.Contains(this);
		}

		public static Point operator +(Point lhs, Size rhs)
		{
			return new Point(lhs.X + rhs.Width, lhs.Y + rhs.Height);
		}

		public static Point operator -(Point lhs, Size rhs)
		{
			return new Point(lhs.X - rhs.Width, lhs.Y - rhs.Height);
		}
	}
}
