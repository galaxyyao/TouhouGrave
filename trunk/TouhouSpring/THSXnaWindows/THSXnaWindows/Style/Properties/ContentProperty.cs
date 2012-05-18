using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Values;

namespace TouhouSpring.Style.Properties
{
	class ContentProperty : BaseProperty
	{
		public interface IHost : IStyleContainer
		{
			int ContentWidth { get; }
			int ContentHeight { get; }
			string DefaultStretch { get; }
			string DefaultHorizontalAlignment { get; }
			string DefaultVerticalAlignment { get; }
			void SetContentBounds(Rectangle bounds, Rectangle contentBounds);
		}

		private string m_stretch;
		private string m_hAlign;
		private string m_vAlign;

		public ContentProperty(IStyleContainer parent)
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
				throw new ArgumentException(String.Format("'{0}' doesn't implement ContentProperty.IHost.", Parent.GetType().Name));
			}

			var contentElement = host.Definition.Element("Content");
			var stretchAttr = contentElement != null ? contentElement.Attribute("Stretch") : null;
			var hAlignAttr = contentElement != null ? contentElement.Attribute("HorizontalAlignment") : null;
			var vAlignAttr = contentElement != null ? contentElement.Attribute("VerticalAlignment") : null;

			var stretch = stretchAttr != null ? stretchAttr.Value : host.DefaultStretch;
			var hAlign = hAlignAttr != null ? hAlignAttr.Value : host.DefaultHorizontalAlignment;
			var vAlign = vAlignAttr != null ? vAlignAttr.Value : host.DefaultVerticalAlignment;

			if (stretch == null)
			{
				throw new MissingAttributeException("Stretch");
			}
			else if (hAlign == null)
			{
				throw new MissingAttributeException("HorizontalAlignment");
			}
			else if (vAlign == null)
			{
				throw new MissingAttributeException("VerticalAlignment");
			}

			m_stretch = stretch;
			m_hAlign = hAlign;
			m_vAlign = vAlign;
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
				throw new ArgumentException(String.Format("'{0}' doesn't implement ContentProperty.IHost.", Parent.GetType().Name));
			}

			Rectangle contentBounds = new Rectangle();

			var stretch = Stretch.Parse(m_stretch);
			switch (stretch.Mode)
			{
				default:
				case StretchMode.None:
					contentBounds.Width = host.ContentWidth;
					contentBounds.Height = host.ContentHeight;
					break;
				case StretchMode.Fill:
					contentBounds.Width = host.Bounds.Width;
					contentBounds.Height = host.Bounds.Height;
					break;
				case StretchMode.Uniform:
				case StretchMode.UniformToFill:
					float k1 = host.Bounds.Width * host.ContentHeight;
					float k2 = host.ContentWidth * host.Bounds.Height;
					float k = stretch.Mode == StretchMode.Uniform ? Math.Min(k1, k2) : Math.Max(k1, k2);
					contentBounds.Width = k / host.ContentHeight;
					contentBounds.Height = k / host.ContentWidth;
					break;
			}

			switch (HAlignment.Parse(m_hAlign).Alignment)
			{
				case HorizontalAlignment.Left:
					contentBounds.Left = 0;
					break;
				default:
				case HorizontalAlignment.Center:
					contentBounds.Left = (host.Bounds.Width - contentBounds.Width) / 2;
					break;
				case HorizontalAlignment.Right:
					contentBounds.Left = host.Bounds.Width - contentBounds.Width;
					break;
			}

			switch (VAlignment.Parse(m_vAlign).Alignment)
			{
				case VerticalAlignment.Top:
					contentBounds.Top = 0;
					break;
				default:
				case VerticalAlignment.Center:
					contentBounds.Top = (host.Bounds.Height - contentBounds.Height) / 2;
					break;
				case VerticalAlignment.Bottom:
					contentBounds.Top = host.Bounds.Height - contentBounds.Height;
					break;
			}

			// bounds
			Point boundsLeftTop = new Point(Math.Max(contentBounds.Left, 0), Math.Max(contentBounds.Top, 0));
			Size boundsSize = new Size(Math.Min(contentBounds.Right, host.Bounds.Width) - boundsLeftTop.X,
									   Math.Min(contentBounds.Bottom, host.Bounds.Height) - boundsLeftTop.Y);

			// content bounds
			Point contentLeftTop = new Point((boundsLeftTop.X - contentBounds.Left) * host.ContentWidth / contentBounds.Width,
											 (boundsLeftTop.Y - contentBounds.Top) * host.ContentHeight / contentBounds.Height);
			Size contentSize = new Size(boundsSize.Width * host.ContentWidth / contentBounds.Width,
										boundsSize.Height * host.ContentHeight / contentBounds.Height);

			host.SetContentBounds(new Rectangle(boundsLeftTop, boundsSize), new Rectangle(contentLeftTop, contentSize));
		}
	}
}
