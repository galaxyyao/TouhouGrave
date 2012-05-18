using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style.Values
{
	enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	struct VAlignment
	{
		public VerticalAlignment Alignment;

		public float ResolveTop(float height, float parentHeight)
		{
			switch (Alignment)
			{
				default:
				case VerticalAlignment.Top:
					return 0;
				case VerticalAlignment.Center:
					return (parentHeight - height) / 2;
				case VerticalAlignment.Bottom:
					return parentHeight - height;
			}
		}

		public static VAlignment Top
		{
			get { return new VAlignment { Alignment = VerticalAlignment.Top }; }
		}

		public static VAlignment Center
		{
			get { return new VAlignment { Alignment = VerticalAlignment.Center }; }
		}

		public static VAlignment Bottom
		{
			get { return new VAlignment { Alignment = VerticalAlignment.Bottom }; }
		}

		public static VAlignment Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			switch (str.ToLower())
			{
				case "top":
					return Top;
				case "center":
					return Center;
				case "bottom":
					return Bottom;
				default:
					throw new FormatException(string.Format("'{0}' is not a valid value for VerticalAlignment.", str));
			}
		}
	}

	static class VAlginmentParser
	{
		public static VAlignment ParseVAlignment(this XElement element, XName attributeName, VAlignment defaultValue)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			else if (attributeName == null)
			{
				throw new ArgumentNullException("attributeName");
			}

			var attrib = element.Attribute(attributeName);
			return attrib != null ? VAlignment.Parse(attrib.Value) : defaultValue;
		}
	}
}
