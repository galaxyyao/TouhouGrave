using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style.Values
{
	enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	struct HAlignment
	{
		public HorizontalAlignment Alignment;

		public float ResolveLeft(float width, float parentWidth)
		{
			switch (Alignment)
			{
				default:
				case HorizontalAlignment.Left:
					return 0;
				case HorizontalAlignment.Center:
					return (parentWidth - width) / 2;
				case HorizontalAlignment.Right:
					return parentWidth - width;
			}
		}

		public static HAlignment Left
		{
			get { return new HAlignment { Alignment = HorizontalAlignment.Left }; }
		}

		public static HAlignment Center
		{
			get { return new HAlignment { Alignment = HorizontalAlignment.Center }; }
		}

		public static HAlignment Right
		{
			get { return new HAlignment { Alignment = HorizontalAlignment.Right }; }
		}

		public static HAlignment Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			switch (str.ToLower())
			{
				case "left":
					return Left;
				case "center":
					return Center;
				case "right":
					return Right;
				default:
					throw new FormatException(string.Format("'{0}' is not a valid value for HorizontalAlignment.", str));
			}
		}
	}

	static class HAlginmentParser
	{
		public static HAlignment ParseHAlignment(this XElement element, XName attributeName, HAlignment defaultValue)
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
			return attrib != null ? HAlignment.Parse(attrib.Value) : defaultValue;
		}
	}
}
