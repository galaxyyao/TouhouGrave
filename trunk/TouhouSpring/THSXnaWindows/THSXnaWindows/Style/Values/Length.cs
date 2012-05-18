using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style.Values
{
	enum LengthUnit
	{
		Pixel,
		Percentage
	}

	struct Length
	{
		public LengthUnit Unit;
		public float Value;

		public static Length Identity
		{
			get { return new Length { Unit = LengthUnit.Percentage, Value = 100 }; }
		}

		public static Length Zero
		{
			get { return new Length { Unit = LengthUnit.Pixel, Value = 0 }; }
		}

		public float Resolve(float referenceLength)
		{
			return Unit == LengthUnit.Percentage ? referenceLength * Value / 100.0f : Value;
		}

		public static Length Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			bool inPercentage = str.EndsWith("%");
			return new Length
			{
				Unit = inPercentage ? LengthUnit.Percentage : LengthUnit.Pixel,
				Value = float.Parse(str.Substring(0, str.Length - (inPercentage ? 1 : 0)))
			};
		}
	}

	static class LengthParser
	{
		public static Length ParseLength(this XElement element, XName attributeName, Length defaultValue)
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
			return attrib != null ? Length.Parse(attrib.Value) : defaultValue;
		}
	}
}
