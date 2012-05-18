using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public struct Size
	{
		public float Width { get; set; }
		public float Height { get; set; }

		public Size(float width, float height) : this()
		{
			Width = width;
			Height = height;
		}

		public static Size operator +(Size lhs, Size rhs)
		{
			return new Size(lhs.Width + rhs.Width, lhs.Height + rhs.Height);
		}

		public static Size operator -(Size lhs, Size rhs)
		{
			return new Size(lhs.Width - rhs.Width, lhs.Height - rhs.Height);
		}

		public static Size operator *(Size lhs, float scale)
		{
			return new Size(lhs.Width * scale, lhs.Height * scale);
		}

		public static Size operator /(Size lhs, float scale)
		{
			return new Size(lhs.Width / scale, lhs.Height / scale);
		}
	}
}
