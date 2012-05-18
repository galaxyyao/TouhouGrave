using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class MouseEventArgs : EventArgs
	{
		public MouseState State
		{
			get; private set;
		}

		public Ray MouseRay
		{
			get; private set;
		}

		public bool Handled
		{
			get; private set;
		}

		public MouseEventArgs(MouseState mouseState, Ray mouseRay)
			: base(EventDispatchOrder.FromTail)
		{
			if (mouseState == null)
			{
				throw new ArgumentNullException("mouseState");
			}

			State = mouseState;
			MouseRay = mouseRay;
			Handled = false;
		}

		public void SetHandled()
		{
			Handled = true;
		}
	}

	class MouseMoveEventArgs : MouseEventArgs
	{
		public MouseMoveEventArgs(MouseState mouseState, Ray mouseRay)
			: base(mouseState, mouseRay)
		{ }
	}

	class MouseButton1EventArgs : MouseEventArgs
	{
		public MouseButton1EventArgs(MouseState mouseState, Ray mouseRay)
			: base(mouseState, mouseRay)
		{ }
	}

	class MouseButton2EventArgs : MouseEventArgs
	{
		public MouseButton2EventArgs(MouseState mouseState, Ray mouseRay)
			: base(mouseState, mouseRay)
		{ }
	}
}
