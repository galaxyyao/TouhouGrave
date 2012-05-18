using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace TouhouSpring.Graphics
{
	public class VirtualTexture
	{
		public int Width
		{
			get { return Bounds.Width; }
		}

		public int Height
		{
			get { return Bounds.Height; }
		}

		public Texture2D XnaTexture
		{
			get; private set;
		}

		public XnaRectangle Bounds
		{
			get; private set;
		}

		public VirtualTexture(Texture2D texture, XnaRectangle uvBounds)
		{
			if (texture == null)
			{
				throw new ArgumentNullException("texture");
			}

			XnaTexture = texture;
			Bounds = uvBounds;
		}
	}
}
