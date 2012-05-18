using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Particle
{
	class SubTextureSelector : TypeConverter
	{
        private List<string> m_cachedUVBounds = new List<string>();
		private string m_cachedTextureName;

        public SubTextureSelector()
        {
            m_cachedUVBounds.Add("{Whole}");
        }

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(GetUVBoundsNames(((Effect)context.Instance).System.TextureName));
		}

		private List<string> GetUVBoundsNames(string name)
		{
			if (name != m_cachedTextureName)
			{
                m_cachedUVBounds.Clear();
                m_cachedUVBounds.Add("{Whole}");

                var atlas = MyResourceLoader.ResolveAtlas(name);
                if (atlas != null)
                {
                    atlas.SubTextures.Keys.ForEach(s => m_cachedUVBounds.Add(s));
                }

                m_cachedTextureName = name;
			}

			return m_cachedUVBounds;
		}
	}
}
