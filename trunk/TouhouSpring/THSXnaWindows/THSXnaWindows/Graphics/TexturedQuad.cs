using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    class TexturedQuad
    {
        public VirtualTexture Texture;
        public Rectangle UVBounds;
        public Color ColorToAdd;
        public Color ColorToModulate;
        public Vector4 ColorScale;
        public BlendState BlendState;
        public bool OffsetByHalfTexel;
        public bool ZTest;
        public bool ZWrite;
        public bool WrapUV;

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
            OffsetByHalfTexel = true;
            ZTest = false;
            ZWrite = false;
            WrapUV = false;
        }
    }
}
