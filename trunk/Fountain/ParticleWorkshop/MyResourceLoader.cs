using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Particle
{
	class MyResourceLoader : IResourceLoader
	{
		private GraphicsDevice m_device;
		private ContentManager m_contentMgr;

		public MyResourceLoader(IServiceProvider services)
		{
			if (services == null)
			{
				throw new ArgumentNullException("services");
			}

			m_device = (services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;
			m_contentMgr = new ContentManager(services, Program.PathUtils.ContentRootDirectory);
		}

		public Texture2D LoadTexture(string name)
		{
			if (name == null)
			{
				return null;
			}

            string atlasPath;
            var atlas = ResolveAtlas(name, out atlasPath);
            if (atlas != null)
            {
                var bmp = TextureAtlas.AtlasWriter.CompositeImage(atlas, Path.GetDirectoryName(atlasPath));
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Texture2D.FromStream(m_device, ms);
                }
            }
            else
            {
                string pngPath = Program.PathUtils.ToDiskPath(name, "png");
                using (FileStream fs = new FileStream(pngPath, FileMode.Open))
                {
                    return Texture2D.FromStream(m_device, fs);
                }
            }
		}

        public XnaRect ResolveUVBounds(string uvBoundsName, ParticleSystem system)
        {
            uvBoundsName = uvBoundsName ?? "{Whole}";
            if (uvBoundsName == "{Whole}")
            {
                return system != null && system.TextureObject != null
                       ? new XnaRect(0, 0, system.TextureObject.Width, system.TextureObject.Height)
                       : XnaRect.Empty;
            }
            else if (uvBoundsName.StartsWith("{") && uvBoundsName.EndsWith("}"))
            {
                string[][] tokens = uvBoundsName.Substring(1, uvBoundsName.Length - 2)
                                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => t.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                int x, y, w, h;
                if (tokens.Length != 4 || tokens.Any(t => t.Length != 2)
                    || tokens[0][0] != "x" || !int.TryParse(tokens[0][1], out x)
                    || tokens[1][0] != "y" || !int.TryParse(tokens[1][1], out y)
                    || (tokens[2][0] != "width" && tokens[2][0] != "w") || !int.TryParse(tokens[2][1], out w)
                    || (tokens[3][0] != "height" && tokens[3][0] != "h") || !int.TryParse(tokens[3][1], out h))
                {
                    throw new FormatException(String.Format("Cannot convert '{0}' to a rectangle.", uvBoundsName));
                }
                return new XnaRect(x, y, w, h);
            }
            else if (system == null)
            {
                return XnaRect.Empty;
            }
            else
            {
                var atlas = ResolveAtlas(system.TextureName);
                if (atlas == null || !atlas.SubTextures.ContainsKey(uvBoundsName))
                {
                    return XnaRect.Empty;
                }

                return atlas.SubTextures[uvBoundsName].Bounds;
            }
        }

		public Curve LoadCurve(string name)
		{
			if (name == null || name == String.Empty)
			{
				return null;
			}

			var path = Program.PathUtils.ToDiskPath(name, "xml");
			using (XmlReader xr = XmlReader.Create(path))
			{
				return IntermediateSerializer.Deserialize<Curve>(xr, Path.GetDirectoryName(path));
			}
		}

		public void Unload(Texture2D texture)
		{
			if (texture != null)
			{
				texture.Dispose();
			}
		}

		public static readonly string AtlasScheme = "atlas:";

        public static TextureAtlas.Atlas ResolveAtlas(string name)
        {
            string path;
            return ResolveAtlas(name, out path);
        }

		public static TextureAtlas.Atlas ResolveAtlas(string name, out string path)
		{
			if (name == null || !name.StartsWith(AtlasScheme))
			{
                path = null;
				return null;
			}
			else
			{
                path = Program.PathUtils.ToDiskPath(name.Substring(AtlasScheme.Length), "xml");
                using (XmlReader xr = XmlReader.Create(path))
                {
                    return IntermediateSerializer.Deserialize<TextureAtlas.Atlas>(xr, Path.GetDirectoryName(path));
                }
			}
		}
	}
}
