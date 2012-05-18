using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Style.Properties
{
	class ImageProperty : BaseProperty
	{
		public interface IHost : IStyleContainer
		{
			string DefaultBlend { get; }
			string DefaultImageUri { get; }
			void SetTexture(string imageUri, BlendState blendState);
		}

		private string m_uri;
		private string m_blend;

		public ImageProperty(IStyleContainer parent)
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
				throw new ArgumentException(String.Format("'{0}' doesn't implement ImageProperty.IHost.", Parent.GetType().Name));
			}

			var uriAttr = host.Definition.Attribute("Uri");
			var uri = uriAttr != null ? uriAttr.Value : host.DefaultImageUri;
			if (uri == null)
			{
				throw new MissingAttributeException("Uri");
			}
			m_uri = uri;

			var blendAttr = host.Definition.Attribute("Blend");
			var blend = blendAttr != null ? blendAttr.Value : host.DefaultBlend;
			if (blend == null)
			{
				throw new MissingAttributeException("Blend");
			}
			m_blend = blend;
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
				throw new ArgumentException(String.Format("'{0}' doesn't implement ImageProperty.IHost.", Parent.GetType().Name));
			}

			BlendState blendState;
			switch (Values.Blend.Parse(BindValuesFor(m_blend)).Mode)
			{
				case Values.BlendMode.Opaque:
					blendState = BlendState.Opaque;
					break;
				default:
				case Values.BlendMode.Alpha:
					blendState = BlendState.AlphaBlend;
					break;
				case Values.BlendMode.Additive:
					blendState = BlendState.Additive;
					break;
			}

			host.SetTexture(BindValuesFor(m_uri), blendState);
		}
	}
}
