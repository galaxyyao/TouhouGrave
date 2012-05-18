using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Particle
{
	public interface IResourceLoader
	{
		Texture2D LoadTexture(string name);
        XnaRect ResolveUVBounds(string uvBoundsName, ParticleSystem system);
		Curve LoadCurve(string name);

		void Unload(Texture2D texture);
	}

	public static class ResourceLoader
	{
		private class DummyResourceLoader : IResourceLoader
		{
			public Texture2D LoadTexture(string name) { return null; }
            public XnaRect ResolveUVBounds(string uvBoundsName, ParticleSystem system) { return XnaRect.Empty; }
			public Curve LoadCurve(string name) { return null; }
			public void Unload(Texture2D texture) { }
		}

		private static IResourceLoader m_dummyLoader = new DummyResourceLoader();
		private static IResourceLoader m_loaderInstance = null;

		public static IResourceLoader Instance
		{
			get { return m_loaderInstance ?? m_dummyLoader; }
			set { m_loaderInstance = value; }
		}
	}
}
