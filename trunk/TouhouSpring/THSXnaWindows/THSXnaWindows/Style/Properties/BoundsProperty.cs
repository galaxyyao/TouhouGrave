using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Values;

namespace TouhouSpring.Style.Properties
{
	class BoundsProperty : BaseProperty
	{
		public interface IHost : IStyleContainer
		{
			string DefaultWidth { get; }
			string DefaultHeight { get; }
			string DefaultHorizontalAlignment { get; }
			string DefaultVerticalAlignment { get; }
			void SetBounds(Rectangle value);
		}

		private string m_left;
		private string m_right;
		private string m_hAlign;
		private string m_width;
		private string m_top;
		private string m_bottom;
		private string m_vAlign;
		private string m_height;

		public BoundsProperty(IStyleContainer parent)
			: base(parent)
		{ }

		public override void Initialize()
		{
			if (Parent.Definition == null)
			{
				return;
			}

			var host = Parent as IHost;
			if (host == null)
			{
				throw new ArgumentException(String.Format("'{0}' doesn't implement BoundsProperty.IHost.", Parent.GetType().Name));
			}

			var leftAttr = host.Definition.Attribute("Left");
			var rightAttr = host.Definition.Attribute("Right");
			var hAlignAttr = host.Definition.Attribute("HorizontalAlignment");
			var widthAttr = host.Definition.Attribute("Width");
			var topAttr = host.Definition.Attribute("Top");
			var bottomAttr = host.Definition.Attribute("Bottom");
			var vAlignAttr = host.Definition.Attribute("VerticalAlignment");
			var heightAttr = host.Definition.Attribute("Height");

			var boundsElement = host.Definition.Element("Bounds");
			if (boundsElement != null)
			{
				var leftAttr2 = boundsElement.Attribute("Left");
				var rightAttr2 = boundsElement.Attribute("Right");
				var hAlignAttr2 = boundsElement.Attribute("HorizontalAlignment");
				var widthAttr2 = boundsElement.Attribute("Width");
				var topAttr2 = boundsElement.Attribute("Top");
				var bottomAttr2 = boundsElement.Attribute("Bottom");
				var vAlignAttr2 = boundsElement.Attribute("VerticalAlignment");
				var heightAttr2 = boundsElement.Attribute("Height");

				if (leftAttr != null && leftAttr2 != null)
				{
					throw new DuplicateAttributeException("Left");
				}
				else if (rightAttr != null && rightAttr2 != null)
				{
					throw new DuplicateAttributeException("Right");
				}
				else if (hAlignAttr != null && hAlignAttr2 != null)
				{
					throw new DuplicateAttributeException("HorizontalAlignment");
				}
				else if (widthAttr != null && widthAttr2 != null)
				{
					throw new DuplicateAttributeException("Width");
				}
				else if (topAttr != null && topAttr2 != null)
				{
					throw new DuplicateAttributeException("Top");
				}
				else if (bottomAttr != null && bottomAttr2 != null)
				{
					throw new DuplicateAttributeException("Bottom");
				}
				else if (vAlignAttr != null && vAlignAttr2 != null)
				{
					throw new DuplicateAttributeException("VerticalAlignment");
				}
				else if (heightAttr != null && heightAttr2 != null)
				{
					throw new DuplicateAttributeException("Height");
				}

				leftAttr = leftAttr ?? leftAttr2;
				rightAttr = rightAttr ?? rightAttr2;
				hAlignAttr = hAlignAttr ?? hAlignAttr2;
				widthAttr = widthAttr ?? widthAttr2;
				topAttr = topAttr ?? topAttr2;
				bottomAttr = bottomAttr ?? bottomAttr2;
				vAlignAttr = vAlignAttr ?? vAlignAttr2;
				heightAttr = heightAttr ?? heightAttr2;
			}

			if (leftAttr != null && rightAttr != null && hAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Left", "Right", "HorizontalAlignment");
			}
			else if (leftAttr != null && hAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Left", "HorizontalAlignment");
			}
			else if (rightAttr != null && hAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Right", "HorizontalAlignment");
			}
			else if (topAttr != null && bottomAttr != null && vAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Top", "Bottom", "VerticalAlignment");
			}
			else if (topAttr != null && vAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Top", "VerticalAlignment");
			}
			else if (bottomAttr != null && vAlignAttr != null)
			{
				throw new MutalExclusiveAttributeException("Bottom", "VerticalAlignment");
			}

			m_left = leftAttr != null ? leftAttr.Value : null;
			m_right = rightAttr != null ? rightAttr.Value : null;
			m_hAlign = hAlignAttr != null ? hAlignAttr.Value : null;
			m_width = widthAttr != null ? widthAttr.Value : null;
			m_top = topAttr != null ? topAttr.Value : null;
			m_bottom = bottomAttr != null ? bottomAttr.Value : null;
			m_vAlign = vAlignAttr != null ? vAlignAttr.Value : null;
			m_height = heightAttr != null ? heightAttr.Value : null;
		}

		public override void Apply()
		{
			if (Parent.Definition == null)
			{
				return;
			}

			var host = Parent as IHost;
			if (host == null)
			{
				throw new ArgumentException(String.Format("'{0}' doesn't implement BoundsProperty.IHost.", Parent.GetType().Name));
			}

			Rectangle bounds = new Rectangle();
			float localLeft = host.Parent != null ? host.Parent.Bounds.Left : 0;
			float localTop = host.Parent != null ? host.Parent.Bounds.Top : 0;
			float parentWidth = host.Parent != null ? host.Parent.Bounds.Width : 0;
			float parentHeight = host.Parent != null ? host.Parent.Bounds.Height : 0;

			var width = m_width ?? host.DefaultWidth;
			if (m_left != null && m_right != null && width != null)
			{
				throw new MutalExclusiveAttributeException("Left", "Right");
			}
			else if (m_hAlign != null && width == null && m_left == null && m_right == null)
			{
				throw new MissingAttributeException("Width");
			}
			else if (width == null && host.Parent == null)
			{
				throw new ApplicationException("Width can't be null because the current style element doesn't have a parent.");
			}
			var hAlign = m_hAlign ?? host.DefaultHorizontalAlignment ?? "Left";

			float? left = null;
			if (m_left != null)
			{
				var leftValue = Length.Parse(m_left);
				if (leftValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Left");
				}
				left = leftValue.Resolve(parentWidth);
			}

			float? right = null;
			if (m_right != null)
			{
				var rightValue = Length.Parse(m_right);
				if (rightValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Right");
				}
				right = rightValue.Resolve(parentWidth);
			}

			if (width != null)
			{
				var widthValue = Length.Parse(width);
				if (widthValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Width");
				}
				bounds.Width = widthValue.Resolve(parentWidth);

				if (left != null)
				{
					bounds.Left = left.Value;
				}
				else if (right != null)
				{
					bounds.Left = parentWidth - bounds.Width - right.Value;
				}
				else
				{
					bounds.Left = host.Parent != null ? HAlignment.Parse(hAlign).ResolveLeft(bounds.Width, parentWidth) : 0;
				}
			}
			else
			{
				bounds.Left = left ?? 0;
				bounds.Width = parentWidth - bounds.Left - (right ?? 0);
			}

			var height = m_height ?? host.DefaultHeight;
			if (m_top != null && m_bottom != null && height != null)
			{
				throw new MutalExclusiveAttributeException("Top", "Bottom");
			}
			else if (m_vAlign != null && height == null && m_top == null && m_bottom == null)
			{
				throw new MissingAttributeException("Height");
			}
			else if (height == null && host.Parent == null)
			{
				throw new ApplicationException("Height can't be null because the current style element doesn't have a parent.");
			}
			var vAlign = m_vAlign ?? host.DefaultVerticalAlignment ?? "Top";

			float? top = null;
			if (m_top != null)
			{
				var topValue = Length.Parse(m_top);
				if (topValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Top");
				}
				top = topValue.Resolve(parentHeight);
			}

			float? bottom = null;
			if (m_bottom != null)
			{
				var bottomValue = Length.Parse(m_bottom);
				if (bottomValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Right");
				}
				bottom = bottomValue.Resolve(parentHeight);
			}

			if (height != null)
			{
				var heightValue = Length.Parse(height);
				if (heightValue.Unit == LengthUnit.Percentage && host.Parent == null)
				{
					throw new CantBeInPercentageException("Height");
				}
				bounds.Height = heightValue.Resolve(parentHeight);

				if (top != null)
				{
					bounds.Top = top.Value;
				}
				else if (bottom != null)
				{
					bounds.Top = parentHeight - bounds.Height - bottom.Value;
				}
				else
				{
					bounds.Top = host.Parent != null ? VAlignment.Parse(vAlign).ResolveTop(bounds.Height, parentHeight) : 0;
				}
			}
			else
			{
				bounds.Top = top ?? 0;
				bounds.Height = parentHeight - bounds.Top - (bottom ?? 0);
			}

			bounds.Left += localLeft;
			bounds.Top += localTop;
			host.SetBounds(bounds);
		}
	}
}
