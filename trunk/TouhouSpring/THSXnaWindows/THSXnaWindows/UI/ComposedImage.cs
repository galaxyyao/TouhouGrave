using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class ComposedImage : TransformNode, IRenderable
	{
		private RenderableProxy m_renderableProxy;

		public struct Quad
		{
			public Graphics.TexturedQuad TextureQuad;
			public Rectangle Bounds;
		}

		public List<Quad> Quads
		{
			get; private set;
		}

		public ComposedImage()
		{
			m_renderableProxy = new RenderableProxy(this);
			Quads = new List<Quad>();
		}

		public void OnRender(RenderEventArgs e)
		{
			var tranform = TransformToGlobal;
			for (int i = 0; i < Quads.Count; ++i)
			{
				var quad = Quads[i];
				e.RenderManager.Draw(quad.TextureQuad, quad.Bounds, tranform);
			}
		}
	}
}
