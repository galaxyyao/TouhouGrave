using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Properties;
using TouhouSpring.Style.Values;
using TouhouSpring.UI;

namespace TouhouSpring.Style
{
	class LayoutGizmo : BaseStyleContainer, BoundsProperty.IHost
	{
		protected TransformNode TypedTarget
		{
			get { return (TransformNode)Target; }
		}

		public LayoutGizmo(IStyleContainer parent, XElement definition)
			: base(parent, definition)
		{ }

		public override void Initialize()
		{
			PreInitialize(() => new UI.TransformNode());

			if (Definition == null)
			{
				return;
			}

			AddChildAndInitialize(new BoundsProperty(this));
			AddChildAndInitialize(new TransformProperty(this));

			foreach (XElement childElement in Definition.Elements())
			{
				if (childElement.Name == "Image")
				{
					AddChildAndInitialize(new ImageStyle(this, childElement));
				}
				else if (childElement.Name == "Label")
				{
					AddChildAndInitialize(new LabelStyle(this, childElement));
				}
				else if (childElement.Name == "Layout")
				{
					AddChildAndInitialize(new LayoutGizmo(this, childElement));
				}
				else if (childElement.Name == "Menu")
				{
					AddChildAndInitialize(new MenuStyle(this, childElement));
				}
                else if (childElement.Name == "Panel")
                {
                    AddChildAndInitialize(new PanelStyle(this, childElement));
                }
			}
		}

		#region BoundsProperty.IHost implementation

		string BoundsProperty.IHost.DefaultWidth { get { return null; } }
		string BoundsProperty.IHost.DefaultHeight { get { return null; } }
		string BoundsProperty.IHost.DefaultHorizontalAlignment { get { return null; } }
		string BoundsProperty.IHost.DefaultVerticalAlignment { get { return null; } }
		void BoundsProperty.IHost.SetBounds(Rectangle value)
		{
			UpdateBounds(value);
		}

		#endregion
	}
}
