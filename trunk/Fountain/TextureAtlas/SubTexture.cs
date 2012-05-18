using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.TextureAtlas
{
    public class SubTexture
    {
        [ContentSerializer(ElementName = "SourceWidth")]
        private int m_sourceWidth;

        [ContentSerializer(ElementName = "SourceHeight")]
        private int m_sourceHeight;

#if WINDOWS
        [System.ComponentModel.ReadOnly(true)]
#endif
        public string SourceFile
        {
            get; set;
        }

        public int Left
        {
            get; set;
        }

        public int Top
        {
            get; set;
        }

        public int Width
        {
            get
            {
                return Rotation == Rotation.Rotation_0 || Rotation == Rotation.Rotation_180
                       ? m_sourceWidth : m_sourceHeight;
            }
        }

        public int Height
        {
            get
            {
                return Rotation == Rotation.Rotation_0 || Rotation == Rotation.Rotation_180
                       ? m_sourceHeight : m_sourceWidth;
            }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public XnaRect Bounds
        {
            get { return new XnaRect(Left, Top, Width, Height); }
        }

        public Rotation Rotation
        {
            get; set;
        }

        public bool FlipHorizontal
        {
            get; set;
        }

        public bool FlipVertical
        {
            get; set;
        }

        public SubTexture(int sourceWidth, int sourceHeight)
        {
            m_sourceWidth = sourceWidth;
            m_sourceHeight = sourceHeight;
        }

        private SubTexture()
        { }
    }
}
