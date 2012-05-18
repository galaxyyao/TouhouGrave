using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Particle
{
	class TextureSelector : TypeConverter
	{
        private List<string> m_textureFiles = new List<string>();

        public TextureSelector()
		{
            m_textureFiles.Add(null);
			foreach (var f in Directory.GetFiles(Program.PathUtils.ContentRootDirectory, "*.xml", SearchOption.AllDirectories))
			{
				var doc = XDocument.Load(f);
				var assetType = doc.Root.Element("Asset");
				if (assetType != null && assetType.Attribute("Type") != null
					&& assetType.Attribute("Type").Value == "TextureAtlas:Atlas")
				{
                    m_textureFiles.Add(MyResourceLoader.AtlasScheme + Program.PathUtils.ToContentAssetUri(f));
				}
			}

            foreach (var png in Directory.GetFiles(Program.PathUtils.ContentRootDirectory, "*.png", SearchOption.AllDirectories))
            {
                m_textureFiles.Add(Program.PathUtils.ToContentAssetUri(png));
            }
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return value ?? "(No Texture)";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				return (string)value != "(No Texture)" ? value : null;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
            return new TypeConverter.StandardValuesCollection(m_textureFiles);
		}
	}
}
