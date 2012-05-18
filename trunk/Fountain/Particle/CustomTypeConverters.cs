using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle
{
#if WINDOWS
	using System.ComponentModel;

	public class CustomTypeConverters
	{
		public static TypeConverter TextureNameProxy
		{
			get; set;
		}

		public static TypeConverter UVBoundsProxy
		{
			get; set;
		}

		public static TypeConverter CurveNameProxy
		{
			get; set;
		}
	}

	class TextureNameTypeConverter : ProxiedCustomTypeConverter
	{
        protected override TypeConverter GetProxy()
        {
            return CustomTypeConverters.TextureNameProxy;
        }
	}

	class UVBoundsTypeConverter : ProxiedCustomTypeConverter
	{
		protected override TypeConverter GetProxy()
		{
			return CustomTypeConverters.UVBoundsProxy;
		}
	}

	class CurveNameTypeConverter : ProxiedCustomTypeConverter
	{
		protected override TypeConverter GetProxy()
		{
			return CustomTypeConverters.CurveNameProxy;
		}
	}

#endif
}
