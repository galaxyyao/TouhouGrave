using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style.Values
{
	enum StretchMode
	{
		// content keeps its original size
		None,
		// content scales to boundary
		Fill,
		// content scales to the smaller size of the two sizes which have only two edges touch the boundary
		// while the aspect ratio is preserved.
		Uniform,
		// content scales to the bigger size of the two sizes which have only two edges touch the boundary
		// while the aspect ratio is preserved.
		UniformToFill,
	}

	struct Stretch
	{
		public StretchMode Mode;

		public static Stretch None
		{
			get { return new Stretch { Mode = StretchMode.None }; }
		}

		public static Stretch Fill
		{
			get { return new Stretch { Mode = StretchMode.Fill }; }
		}

		public static Stretch Uniform
		{
			get { return new Stretch { Mode = StretchMode.Uniform }; }
		}

		public static Stretch UniformToFill
		{
			get { return new Stretch { Mode = StretchMode.UniformToFill }; }
		}

		public static Stretch Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			switch (str.ToLower())
			{
				case "none":
					return None;
				case "fill":
					return Fill;
				case "uniform":
					return Uniform;
				case "uniformtofill":
					return UniformToFill;
				default:
					throw new FormatException(string.Format("'{0}' is not a valid value for Stretch.", str));
			}
		}
	}

	static class StretchParser
	{
		public static Stretch ParseStretch(this XElement element, XName attributeName, Stretch defaultValue)
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
			return attrib != null ? Stretch.Parse(attrib.Value) : defaultValue;
		}
	}
}
