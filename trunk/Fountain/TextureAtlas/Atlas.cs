using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.TextureAtlas
{
    public class Atlas : IDisposable
    {
        private Dictionary<string, SubTexture> m_subTextures;

        [ContentSerializer(ElementName = "Texture", AllowNull = true)]
        private Texture2D m_texture = null;

        public Dictionary<string, SubTexture> SubTextures
        {
            get { return m_subTextures; }
        }

        public int Width
        {
            get; set;
        }

        public int Height
        {
            get; set;
        }

        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return m_texture; }
        }

        public Atlas()
        {
            m_subTextures = new Dictionary<string, SubTexture>();
        }

        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Atlas()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing && m_texture != null)
                {
                    // TODO: check to see whether it is a double disposal
                    m_texture.Dispose();
                }

                m_texture = null;
                m_disposed = true;
            }
        }
    }
}
