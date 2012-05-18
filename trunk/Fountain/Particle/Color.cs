using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Particle
{
	/// <summary>
	/// In document it says it should be in ARGB format, but actually it is in ABGR format.
	/// </summary>

#if WINDOWS
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
#endif
	public struct Color
	{
		public UInt32 underlay;

		public const UInt32 MaskA = 0xff000000;
		public const UInt32 MaskB = 0x00ff0000;
		public const UInt32 MaskG = 0x0000ff00;
		public const UInt32 MaskR = 0x000000ff;
		public const int ShiftA = 24;
		public const int ShiftB = 16;
		public const int ShiftG = 8;
		public const int ShiftR = 0;

		public byte A
		{
			get { return (byte)((underlay & MaskA) >> ShiftA); }
			set { underlay = (underlay & ~MaskA) | (UInt32)(value << ShiftA); }
		}

		public byte B
		{
			get { return (byte)((underlay & MaskB) >> ShiftB); }
			set { underlay = (underlay & ~MaskB) | (UInt32)(value << ShiftB); }
		}

		public byte G
		{
			get { return (byte)((underlay & MaskG) >> ShiftG); }
			set { underlay = (underlay & ~MaskG) | (UInt32)(value << ShiftG); }
		}

		public byte R
		{
			get { return (byte)((underlay & MaskR) >> ShiftR); }
			set { underlay = (underlay & ~MaskR) | (UInt32)(value << ShiftR); }
		}

		public Color(byte r, byte g, byte b, byte a)
		{
			underlay = (UInt32)(a << 24) | (UInt32)(b << 16) | (UInt32)(g << 8) | r;
		}

		public XnaColor ToXnaColor()
		{
			return new XnaColor(R, G, B, A);
		}

		public static Color FromXnaColor(XnaColor xnaColor)
		{
			return new Color(xnaColor.R, xnaColor.G, xnaColor.B, xnaColor.A);
		}
	}
}
