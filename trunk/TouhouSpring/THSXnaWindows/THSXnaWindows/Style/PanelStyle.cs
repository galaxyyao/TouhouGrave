using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Style.Properties;
using System.Xml.Linq;
using TouhouSpring.UI;

namespace TouhouSpring.Style
{
    class PanelStyle : BaseStyleContainer, BoundsProperty.IHost
    {
        public Panel TypedTarget
        {
            get { return (Panel)Target; }
        }

        public override Rectangle Bounds
        {
            get { return TypedTarget.Region; }
            protected set { TypedTarget.Region = value; }
        }

        public PanelStyle(IStyleContainer parent, XElement definition)
			: base(parent, definition)
		{ }

        public override void Initialize()
        {
            PreInitialize(() => new Panel());
            if (Definition == null)
            {
                return;
            }
            AddChildAndInitialize(new BoundsProperty(this));
            AddChildAndInitialize(new TransformProperty(this));
        }

        #region BoundsProperty.IHost implementation

        string BoundsProperty.IHost.DefaultWidth { get { return Parent.Bounds.Width.ToString(); } }
        string BoundsProperty.IHost.DefaultHeight { get { return Parent.Bounds.Height.ToString(); } }
        string BoundsProperty.IHost.DefaultHorizontalAlignment { get { return null; } }
        string BoundsProperty.IHost.DefaultVerticalAlignment { get { return null; } }
        void BoundsProperty.IHost.SetBounds(Rectangle value)
        {
            UpdateBounds(value);
        }

        #endregion
    }
}
