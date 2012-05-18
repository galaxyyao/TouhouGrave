using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace TouhouSpring.UI
{
	interface ITransformNode
	{
		XnaMatrix Transform { get; set; }
		XnaMatrix TransformToGlobal { get; }
		ITransformNode ParentNode { get; }
	}

	class TransformNode : EventDispatcher, ITransformNode
	{
		public virtual XnaMatrix Transform
		{
			get; set;
		}

		public ITransformNode ParentNode
		{
			get
			{
				for (var dispatcher = Dispatcher; dispatcher != null; dispatcher = dispatcher.Dispatcher)
				{
					if (dispatcher is ITransformNode)
					{
						return dispatcher as ITransformNode;
					}
				}
				return null;
			}
		}

		public XnaMatrix TransformToGlobal
		{
			get
			{
				XnaMatrix t = Transform;
				for (ITransformNode node = ParentNode; node != null; node = node.ParentNode)
				{
					t *= node.Transform;
				}
				return t;
			}
		}

		public TransformNode()
		{
			Transform = XnaMatrix.Identity;
		}

		public static XnaMatrix GetTransformBetween(EventDispatcher from, EventDispatcher to)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			else if (to == null)
			{
				throw new ArgumentNullException("to");
			}

			if (from == to)
			{
				return XnaMatrix.Identity;
			}

			// construct a path from current dispatcher to root (null)
			List<EventDispatcher> path = new List<EventDispatcher>();
			for (var i = from; i != null; i = i.Dispatcher)
			{
				path.Add(i);
			}

			EventDispatcher commonAncestor = null;
			// traverse from the target dispatcher up to meet the first dispatcher in the path
			for (var i = to; i != null; i = i.Dispatcher)
			{
				if (path.Contains(i))
				{
					commonAncestor = i;
					break;
				}
			}
			if (commonAncestor == null)
			{
				throw new InvalidOperationException("Dispatcher is not in the same tree.");
			}

			// get the transform from current dispatcher to common ancestor
			XnaMatrix transform1 = XnaMatrix.Identity;
			for (var i = from; i != commonAncestor; i = i.Dispatcher)
			{
				if (i is ITransformNode)
				{
					transform1 *= (i as ITransformNode).Transform;
				}
			}
			// get the transform from target dispatcher to common ancestor
			XnaMatrix transform2 = XnaMatrix.Identity;
			for (var i = to; i != commonAncestor; i = i.Dispatcher)
			{
				if (i is ITransformNode)
				{
					transform2 *= (i as ITransformNode).Transform;
				}
			}

			return transform1 * transform2.Invert();
		}
	}
}
