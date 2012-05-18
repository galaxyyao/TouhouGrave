using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Style.Values
{
	enum BlendMode
	{
		Opaque,
		Alpha,
		Additive
	}

	struct Blend
	{
		public BlendMode Mode;

		public static Blend Opaque
		{
			get { return new Blend { Mode = BlendMode.Opaque }; }
		}

		public static Blend Alpha
		{
			get { return new Blend { Mode = BlendMode.Alpha }; }
		}

		public static Blend Additive
		{
			get { return new Blend { Mode = BlendMode.Additive }; }
		}

		public static Blend Parse(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			switch (str.ToLower())
			{
				case "opaque":
					return Opaque;
				case "alpha":
					return Alpha;
				case "additive":
					return Additive;
				default:
					throw new FormatException(string.Format("'{0}' is not a valid value for Blend.", str));
			}
		}
	}
}
