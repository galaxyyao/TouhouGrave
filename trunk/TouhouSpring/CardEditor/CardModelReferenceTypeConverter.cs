using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    class CardModelReferenceTypeConverter : TypeConverter
    {
        private Func<IEnumerable<ICardModel>> m_cardModelIterator;

        public CardModelReferenceTypeConverter(Func<IEnumerable<ICardModel>> iterator)
        {
            m_cardModelIterator = iterator;
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
                if (value is string)
                {
                    return value;
                }

                var cmRef = value as CardModelReference;

                return cmRef == null || cmRef.Target == null
                       ? "{null}"
                       : cmRef.Target.Id;
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
            if (value is string)
            {
                var str = value as string;
                if (str == "{null}")
                {
                    return null;
                }

                var cardModel = m_cardModelIterator().FirstOrDefault(cm => cm.Id == str);
                return cardModel != null ? new CardModelReference { Target = cardModel } : null;
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
            return new StandardValuesCollection(IterateOptions().ToArray());
        }

        private IEnumerable<string> IterateOptions()
        {
            yield return null;
            foreach (var cm in m_cardModelIterator())
            {
                yield return cm.Id;
            }
        }
    }
}
