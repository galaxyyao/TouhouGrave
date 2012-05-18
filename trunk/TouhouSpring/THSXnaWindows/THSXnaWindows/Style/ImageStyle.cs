using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using TouhouSpring.Style.Properties;
using TouhouSpring.Style.Values;
using ComposedImage = TouhouSpring.UI.ComposedImage;

namespace TouhouSpring.Style
{
	class ImageStyle : BaseStyleContainer, BoundsProperty.IHost, ContentProperty.IHost, ImageProperty.IHost
	{
		private string m_imageUri = null;

		public ComposedImage TypedTarget
		{
			get { return (ComposedImage)Target; }
		}

		public ImageStyle(IStyleContainer parent, XElement definition)
			: base(parent, definition)
		{ }

		public override void Initialize()
		{
			PreInitialize(() => new ComposedImage());

			if (Definition == null)
			{
				return;
			}

			AddChildAndInitialize(new ImageProperty(this));
			AddChildAndInitialize(new BoundsProperty(this));
			AddChildAndInitialize(new TransformProperty(this));
			AddChildAndInitialize(new ContentProperty(this));
		}

		#region BoundsProperty.IHost implementation

		string BoundsProperty.IHost.DefaultWidth { get { return TypedTarget.Quads[0].TextureQuad.Texture.Width.ToString(); } }
		string BoundsProperty.IHost.DefaultHeight { get { return TypedTarget.Quads[0].TextureQuad.Texture.Height.ToString(); } }
		string BoundsProperty.IHost.DefaultHorizontalAlignment { get { return null; } }
		string BoundsProperty.IHost.DefaultVerticalAlignment { get { return null; } }
		void BoundsProperty.IHost.SetBounds(Rectangle value)
		{
			UpdateBounds(value);
		}

		#endregion

		#region ContentProperty.IHost implementation

		int ContentProperty.IHost.ContentWidth { get { return TypedTarget.Quads[0].TextureQuad.Texture.Width; } }
		int ContentProperty.IHost.ContentHeight { get { return TypedTarget.Quads[0].TextureQuad.Texture.Height; } }
		string ContentProperty.IHost.DefaultStretch { get { return "None"; } }
		string ContentProperty.IHost.DefaultHorizontalAlignment { get { return "Center"; } }
		string ContentProperty.IHost.DefaultVerticalAlignment { get { return "Center"; } }
		void ContentProperty.IHost.SetContentBounds(Rectangle bounds, Rectangle contentBounds)
		{
			var quad = TypedTarget.Quads[0];
			quad.TextureQuad.UVBounds = contentBounds;
			quad.Bounds = bounds;
			TypedTarget.Quads[0] = quad;
		}

		#endregion

		#region ImageProperty.IHost implementation

		string ImageProperty.IHost.DefaultBlend { get { return "Alpha"; } }
		string ImageProperty.IHost.DefaultImageUri { get { return null; } }
		void ImageProperty.IHost.SetTexture(string imageUri, BlendState blendState)
		{
			if (TypedTarget.Quads.Count > 0 && imageUri == m_imageUri)
			{
				TypedTarget.Quads[0].TextureQuad.BlendState = blendState;
				return;
			}

			IResourceContainer resourceContainer = null;
			for (var t = Target; t != null; t = t.Dispatcher)
			{
				if (t is IResourceContainer)
				{
					resourceContainer = t as IResourceContainer;
					break;
				}
			}

			if (TypedTarget.Quads.Count > 0)
			{
				if (resourceContainer != null)
				{
					resourceContainer.Release(TypedTarget.Quads[0].TextureQuad.Texture);
				}
				TypedTarget.Quads.Clear();
			}

			var texture = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>(imageUri);
			if (resourceContainer != null)
			{
				resourceContainer.Register(texture);
			}
			var texQuad = new Graphics.TexturedQuad(texture) { BlendState = blendState };
			TypedTarget.Quads.Add(new ComposedImage.Quad { TextureQuad = texQuad });
			m_imageUri = imageUri;
		}

		#endregion
	}
}
