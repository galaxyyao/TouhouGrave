using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SystemColor = System.Drawing.Color;

namespace TouhouSpring.Style.Values
{
	struct Color
	{
		public byte Red;
		public byte Green;
		public byte Blue;
		public byte Alpha;

		public static Color Black
		{
			get { return new Color { Red = 0, Green = 0, Blue = 0, Alpha = 255 }; }
		}

		public static Color White
		{
			get { return new Color { Red = 255, Green = 255, Blue = 255, Alpha = 255 }; }
		}

		public static Color Transparent
		{
			get { return new Color { Red = 0, Green = 0, Blue = 0, Alpha = 0 }; }
		}

		public static Color Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			if (str.StartsWith("!"))
			{
				var hexStr = str.Substring(1);
				int hexNum;
				if (hexStr.Length != 6 && hexStr.Length != 8
					|| !int.TryParse(hexStr, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out hexNum))
				{
					throw new FormatException(String.Format("'{0}' is not a valid value for Color.", str));
				}

				if (hexStr.Length == 6)
				{
					// RGB
					return new Color
					{
						Red = (byte)((hexNum >> 16) & 0xff),
						Green = (byte)((hexNum >> 8) & 0xff),
						Blue = (byte)((hexNum >> 0) & 0xff),
						Alpha = 255
					};
				}
				else
				{
					// RGBA
					return new Color
					{
						Red = (byte)((hexNum >> 24) & 0xff),
						Green = (byte)((hexNum >> 16) & 0xff),
						Blue = (byte)((hexNum >> 8) & 0xff),
						Alpha = (byte)((hexNum >> 0) & 0xff)
					};
				}
			}
			else
			{
				SystemColor namedColor = SystemColor.FromName(str);
				return new Color { Red = namedColor.R, Green = namedColor.G, Blue = namedColor.B, Alpha = namedColor.A };
			}
		}
	}

	static class ColorParser
	{
		public static Color ParseColor(this XElement element, XName attributeName, Color defaultValue)
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
			return attrib != null ? Color.Parse(attrib.Value) : defaultValue;
		}
	}
}
