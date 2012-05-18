using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public struct Rectangle
	{
		public Point LeftTop
		{
			get; set;
		}

		public Size Size
		{
			get; set;
		}

		public float Left
		{
			get { return LeftTop.X; }
			set { LeftTop = new Point(value, Top); }
		}

		public float Top
		{
			get { return LeftTop.Y; }
			set { LeftTop = new Point(Left, value); }
		}

		public float Right
		{
			get { return LeftTop.X + Size.Width; }
		}

		public float Bottom
		{
			get { return LeftTop.Y + Size.Height; }
		}

		public float Width
		{
			get { return Size.Width; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Width value must be non-negative.");
				}
				Size = new Size(value, Height);
			}
		}

		public float Height
		{
			get { return Size.Height; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Height value must be non-negative.");
				}
				Size = new Size(Width, value);
			}
		}

		public Rectangle(Point leftTop, Size size) : this()
		{
			LeftTop = leftTop;
			Size = size;
		}

		public Rectangle(float left, float top, float width, float height) : this()
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public bool Contains(Point pt)
		{
			return pt.X >= Left && pt.X < Right && pt.Y >= Top && pt.Y < Bottom;
		}

		public bool Contains(Rectangle rectangle)
		{
			return rectangle.Left >= Left && rectangle.Top >= Top && rectangle.Right <= Right && rectangle.Bottom <= Bottom;
		}

		public bool Overlaps(Rectangle rectangle)
		{
			return Left < rectangle.Right && rectangle.Left < Right && Top < rectangle.Bottom && rectangle.Top < Bottom;
		}
	}
}
