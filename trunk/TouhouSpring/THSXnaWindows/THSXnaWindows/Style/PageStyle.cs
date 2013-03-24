﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Properties;
using TouhouSpring.UI;

namespace TouhouSpring.Style
{
    class PageStyle : BaseStyleContainer
    {
        public Page TypedTarget
        {
            get { return (Page)Target; }
        }

        public IBindingProvider BindingProvider
        {
            get; set;
        }

        public override Rectangle Bounds
        {
            get { return new Rectangle(0, 0, TypedTarget.Width, TypedTarget.Height); }
            protected set { throw new NotSupportedException(); }
        }

        public override IEnumerable<IBindingProvider> BindingProviders
        {
            get
            {
                return BindingProvider != null
                       ? Enumerable.Repeat(BindingProvider, 1)
                       : base.BindingProviders;
            }
        }

        public PageStyle(XElement definition)
            : base(null, definition)
        { }

        public override void Initialize()
        {
            PreInitialize(() => new Page(this));

            if (Definition == null)
            {
                return;
            }

            foreach (var childElement in Definition.Elements())
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
                    AddChildAndInitialize(new LayoutGizmo<UI.TransformNode>(this, childElement));
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
    }
}
