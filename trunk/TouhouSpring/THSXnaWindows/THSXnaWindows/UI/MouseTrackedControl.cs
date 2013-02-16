using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
	class MouseTrackedControl : TransformNode, IMouseTracked
	{
		public Rectangle Region
		{
			get; set;
		}

		public bool IsMouseOver
		{
			get { return MouseTracked.MouseEntered; }
		}

		public bool IsMouseButton1Pressed
		{
			get { return MouseTracked.MouseButton1Pressed; }
		}

		public bool IsMouseButton2Pressed
		{
			get { return MouseTracked.MouseButton2Pressed; }
		}

		protected MouseTracked MouseTracked
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

		protected MouseTrackedControl()
		{
			MouseTracked = new MouseTracked(this);
			MouseTracked.Dispatcher = this;

			MouseTracked.MouseMove += (sender, e) => RaiseEventHandler(MouseMove, e);
			MouseTracked.MouseEnter += (sender, e) => RaiseEventHandler(MouseEnter, e);
			MouseTracked.MouseLeave += (sender, e) => RaiseEventHandler(MouseLeave, e);
			MouseTracked.MouseButton1Down += (sender, e) => RaiseEventHandler(MouseButton1Down, e);
			MouseTracked.MouseButton1Up += (sender, e) => RaiseEventHandler(MouseButton1Up, e);
			MouseTracked.MouseButton2Down += (sender, e) => RaiseEventHandler(MouseButton2Down, e);
			MouseTracked.MouseButton2Up += (sender, e) => RaiseEventHandler(MouseButton2Up, e);
		}

		public bool IntersectWith(Ray ray)
		{
			return MouseTracked.Intersect(ray, Region, TransformToGlobal.Invert());
		}

		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnMouseEnter(MouseEventArgs e) { }
		public virtual void OnMouseLeave(MouseEventArgs e) { }
		public virtual void OnMouseButton1Down(MouseEventArgs e) { }
		public virtual void OnMouseButton1Up(MouseEventArgs e) { }
		public virtual void OnMouseButton2Down(MouseEventArgs e) { }
		public virtual void OnMouseButton2Up(MouseEventArgs e) { }

		private void RaiseEventHandler<T>(EventHandler<T> eventHandler, T e)
			where T : System.EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}
	}
}
