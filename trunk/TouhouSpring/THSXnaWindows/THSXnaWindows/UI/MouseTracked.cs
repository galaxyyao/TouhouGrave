using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
	interface IMouseTracked
	{
		bool IntersectWith(Ray ray);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseEnter(MouseEventArgs e);
		void OnMouseLeave(MouseEventArgs e);
		void OnMouseButton1Down(MouseEventArgs e);
		void OnMouseButton1Up(MouseEventArgs e);
		void OnMouseButton2Down(MouseEventArgs e);
		void OnMouseButton2Up(MouseEventArgs e);
	}

	class MouseTracked : EventListener,
		IEventListener<MouseMoveEventArgs>,
		IEventListener<MouseButton1EventArgs>,
		IEventListener<MouseButton2EventArgs>
	{
		public bool MouseEntered
		{
			get; private set;
		}

		public bool MouseButton1Pressed
		{
			get; private set;
		}

		public bool MouseButton2Pressed
		{
			get; private set;
		}

		public IMouseTracked EventTarget
		{
			get; private set;
		}

		public event EventHandler<MouseEventArgs> MouseMove;
		public event EventHandler<MouseEventArgs> MouseEnter;
		public event EventHandler<MouseEventArgs> MouseLeave;
		public event EventHandler<MouseEventArgs> MouseButton1Down;
		public event EventHandler<MouseEventArgs> MouseButton1Up;
		public event EventHandler<MouseEventArgs> MouseButton2Down;
		public event EventHandler<MouseEventArgs> MouseButton2Up;

		public MouseTracked(IMouseTracked target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			EventTarget = target;
		}

		public void RaiseEvent(MouseMoveEventArgs e)
		{
			bool onControl = !e.Handled && EventTarget.IntersectWith(e.MouseRay);
			if (onControl && !MouseEntered)
			{
				EventTarget.OnMouseEnter(e);
				RaiseEventHandler(MouseEnter, e);
			}
			else if (!onControl && MouseEntered)
			{
				EventTarget.OnMouseLeave(e);
				RaiseEventHandler(MouseLeave, e);
			}

			MouseEntered = onControl;

			EventTarget.OnMouseMove(e);
			RaiseEventHandler(MouseMove, e);

			if (onControl)
			{
				e.SetHandled();
			}
		}

		public void RaiseEvent(MouseButton1EventArgs e)
		{
			bool onControl = !e.Handled && EventTarget.IntersectWith(e.MouseRay);

			if (onControl)
			{
				// mouse down events are fired only if on control
				if (!MouseButton1Pressed && e.State.Button1Pressed)
				{
					MouseButton1Pressed = true;
					EventTarget.OnMouseButton1Down(e);
					RaiseEventHandler(MouseButton1Down, e);
				}
			}

			if (MouseButton1Pressed && !e.State.Button1Pressed)
			{
				MouseButton1Pressed = false;
				EventTarget.OnMouseButton1Up(e);
				RaiseEventHandler(MouseButton1Up, e);
			}

			if (onControl)
			{
				e.SetHandled();
			}
		}

		public void RaiseEvent(MouseButton2EventArgs e)
		{
			bool onControl = !e.Handled && EventTarget.IntersectWith(e.MouseRay);

			if (onControl)
			{
				// mouse down events are fired only if on control
				if (!MouseButton2Pressed && e.State.Button2Pressed)
				{
					MouseButton2Pressed = true;
					EventTarget.OnMouseButton2Down(e);
					RaiseEventHandler(MouseButton2Down, e);
				}
			}

			if (MouseButton2Pressed && !e.State.Button2Pressed)
			{
				MouseButton2Pressed = false;
				EventTarget.OnMouseButton2Up(e);
				RaiseEventHandler(MouseButton2Up, e);
			}

			if (onControl)
			{
				e.SetHandled();
			}
		}

		private void RaiseEventHandler(EventHandler<MouseEventArgs> eventHandler, MouseEventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public static bool Intersect(Ray ray, Rectangle rectXY, Matrix globalToLocal)
		{
			ray = globalToLocal.Transform(ray);
			if (Math.Abs(ray.Direction.Z) < 0.0001f)
			{
				return false;
			}

			float d = -ray.Origin.Z / ray.Direction.Z;
			return rectXY.Contains(new Point(ray.Origin.X + d * ray.Direction.X, ray.Origin.Y + d * ray.Direction.Y));
		}
	}
}
