using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    [Flags]
    enum TextureQuadFlags
    {
        OffsetByHalfTexel   = 0x1 << 0,
        ZTest               = 0x1 << 1,
        ZWrite              = 0x1 << 2,
        WrapUV              = 0x1 << 3,
        SharpLod            = 0x1 << 4
    }

    class TexturedQuad
    {

        public VirtualTexture Texture;
        public Rectangle UVBounds;
        public Color ColorToAdd;
        public Color ColorToModulate;
        public Vector4 ColorScale;
        public BlendState BlendState;
        public TextureQuadFlags Flags;

        public TexturedQuad() : this(null)
        { }

        public TexturedQuad(VirtualTexture texture)
        {
            Texture = texture;
            UVBounds = new Rectangle(0, 0, texture != null ? texture.Width : 0, texture != null ? texture.Height : 0);
            ColorToAdd = Color.Transparent;
            ColorToModulate = Color.White;
            ColorScale = Vector4.One;
            BlendState = BlendState.AlphaBlend;
            Flags |= TextureQuadFlags.OffsetByHalfTexel;
        }
    }
}
