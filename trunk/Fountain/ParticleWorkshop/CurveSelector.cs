using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Particle
{
	class CurveSelector : TypeConverter
	{
		private List<string> m_curveFiles = new List<string>();

		public CurveSelector()
		{
			m_curveFiles.Add(null);
			foreach (var f in Directory.GetFiles(Program.PathUtils.ContentRootDirectory, "*.xml", SearchOption.AllDirectories))
			{
				var doc = XDocument.Load(f);
				var assetType = doc.Root.Element("Asset");
				if (assetType != null && assetType.Attribute("Type") != null
					&& assetType.Attribute("Type").Value == "Framework:Curve")
				{
					m_curveFiles.Add(Program.PathUtils.ToContentAssetUri(f));
				}
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
				return value ?? "(No Curve)";
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
				return (string)value != "(No Curve)" ? value : null;
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
			return new StandardValuesCollection(m_curveFiles);
		}
	}
}
