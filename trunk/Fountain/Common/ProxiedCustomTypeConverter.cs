using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    using System.ComponentModel;

    public class ProxiedCustomTypeConverter : TypeConverter
    {
        protected virtual TypeConverter GetProxy() { return null; }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (GetProxy() != null)
            {
                return GetProxy().CanConvertFrom(context, sourceType);
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (GetProxy() != null)
            {
                return GetProxy().ConvertFrom(context, culture, value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (GetProxy() != null)
            {
                return GetProxy().CanConvertTo(context, destinationType);
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (GetProxy() != null)
            {
                return GetProxy().ConvertTo(context, culture, value, destinationType);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (GetProxy() != null)
            {
                return GetProxy().GetStandardValuesSupported(context);
            }
            return base.GetStandardValuesSupported(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            if (GetProxy() != null)
            {
                return GetProxy().GetStandardValuesExclusive(context);
            }
            return base.GetStandardValuesExclusive(context);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (GetProxy() != null)
            {
                return GetProxy().GetStandardValues(context);
            }
            return base.GetStandardValues(context);
        }
    }
}
