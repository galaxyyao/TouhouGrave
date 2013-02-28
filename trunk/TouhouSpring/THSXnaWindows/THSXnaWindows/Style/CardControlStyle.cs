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
	class CardControlStyle : BaseStyleContainer, BoundsProperty.IHost
	{
		private BaseCard m_card;
		private List<IBindingProvider> m_bindingProviders = new List<IBindingProvider>();

		public CardControl TypedTarget
		{
			get { return (CardControl)Target; }
		}

		public override IEnumerable<IBindingProvider> BindingProviders
		{
			get { return m_bindingProviders; }
		}

		public override Rectangle Bounds
		{
			get { return TypedTarget.Region; }
			protected set { TypedTarget.Region = value; }
		}

		public CardControlStyle(XElement definition, BaseCard cardToBind)
			: base(null, definition)
		{
			if (cardToBind == null)
			{
				throw new ArgumentNullException("cardToBind");
			}

			m_card = cardToBind;
		}

		public void RegisterBinding(IBindingProvider bindingProvider)
		{
			if (bindingProvider == null)
			{
				throw new ArgumentNullException("bindingProvider");
			}
			else if (m_bindingProviders.Contains(bindingProvider))
			{
				throw new ArgumentException("BindingProvider has already been registered.");
			}

			m_bindingProviders.Insert(0, bindingProvider);
		}

		public override void Initialize()
		{
			PreInitialize(() => new CardControl(m_card, this));

			if (Definition == null)
			{
				return;
			}

			AddChildAndInitialize(new BoundsProperty(this));
			AddChildAndInitialize(new TransformProperty(this));

			foreach (XElement childElement in Definition.Elements())
			{
				if (childElement.Name == "Layout")
				{
					AddChildAndInitialize(new LayoutGizmo(this, childElement));
				}
				else if (childElement.Name == "Image")
				{
					AddChildAndInitialize(new ImageStyle(this, childElement));
				}
				else if (childElement.Name == "Label")
				{
					AddChildAndInitialize(new LabelStyle(this, childElement));
				}
			}

			TypedTarget.OnStyleInitialized();
		}

		#region BoundsProperty.IHost implementation

		string BoundsProperty.IHost.DefaultWidth { get { return null; } }
		string BoundsProperty.IHost.DefaultHeight { get { return null; } }
		string BoundsProperty.IHost.DefaultHorizontalAlignment { get { return null; } }
		string BoundsProperty.IHost.DefaultVerticalAlignment { get { return null; } }
		void BoundsProperty.IHost.SetBounds(Rectangle value)
		{
			Bounds = new Rectangle(0, 0, value.Width, value.Height);
			TypedTarget.Transform = MatrixHelper.Identity;
		}

		#endregion
	}
}
