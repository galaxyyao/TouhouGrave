using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class MouseState
	{
		public Point Location
		{
			get; private set;
		}

		public bool Button1Pressed
		{
			get; private set;
		}

		public bool Button2Pressed
		{
			get; private set;
		}

		public MouseState(float X, float Y, bool btn1Pressed, bool btn2Pressed)
		{
			Location = new Point(X, Y);
			Button1Pressed = btn1Pressed;
			Button2Pressed = btn2Pressed;
		}
	}
}
