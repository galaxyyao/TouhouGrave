using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Graphics
{
	class TextureAtlasManager : Services.ResourceManager.IManager
	{
		public bool IsRelevant(Type resourceType)
		{
			if (resourceType == null)
			{
				throw new ArgumentNullException("resourceType");
			}

			return resourceType == typeof(VirtualTexture);
		}

		public T Load<T>(string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}

			const string Scheme = "atlas:";

			if (!uri.StartsWith(Scheme, true, null))
			{
				var texture = GameApp.Service<Services.ResourceManager>().Acquire<Texture2D>(uri);
				return (T)(object)new VirtualTexture(texture, texture.Bounds);
			}

			int sep = uri.LastIndexOf('#');
			string atlasName = uri.Substring(Scheme.Length, (sep != -1 ? sep : uri.Length) - Scheme.Length);
			string subTextureId = sep != -1 ? uri.Substring(sep + 1) : null;

            var atlas = GameApp.Service<Services.ResourceManager>().Acquire<TextureAtlas.Atlas>(atlasName);
            if (subTextureId == null)
            {
                return (T)(object)new Graphics.VirtualTexture(atlas.Texture, new XnaRectangle(0, 0, atlas.Texture.Width, atlas.Texture.Height));
            }
            else
            {
                var subTexture = atlas.SubTextures[subTextureId];
                return (T)(object)new Graphics.VirtualTexture(atlas.Texture, subTexture.Bounds);
            }
		}

		public void Unload(object resource)
		{
			var tex = (VirtualTexture)resource;
			GameApp.Service<Services.ResourceManager>().Release(tex.XnaTexture);
		}

		public void Dispose()
		{
		}
	}
}
