using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Graphics
{
	partial class ParticleRenderer
	{
		private class ResourceLoader : Particle.IResourceLoader
		{
			public Texture2D LoadTexture(string assetName)
			{
				if (assetName == null)
				{
					return null;
				}

				return GameApp.Service<Services.ResourceManager>().Acquire<VirtualTexture>(assetName).XnaTexture;
			}

            public XnaRect ResolveUVBounds(string uvBoundsName, Particle.ParticleSystem system)
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
                else
                {
                    return XnaRect.Empty;
                }
            }

			public Curve LoadCurve(string assetName)
			{
				if (assetName == null)
				{
					return null;
				}

				return GameApp.Service<Services.ResourceManager>().Acquire<Curve>(assetName);
			}

			public void Unload(Texture2D texture)
			{
				if (texture != null)
				{
					GameApp.Service<Services.ResourceManager>().Release(texture);
				}
			}
		}
	}
}
