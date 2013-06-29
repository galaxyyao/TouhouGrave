using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    class BehaviorModelReferenceTypeConverter : TypeConverter
    {
        private Func<IEnumerable<Type>> m_bhvModelTypeIterator;

        public static string GetBehaviorName(Type behaviorModelType)
        {
            if (behaviorModelType == null)
            {
                throw new ArgumentNullException("behaviorModelType");
            }
            var bhvAttr = behaviorModelType.GetAttribute<Behaviors.BehaviorModelAttribute>();
            if (bhvAttr == null || bhvAttr.HideFromEditor)
            {
                throw new ArgumentException("behaviorModelType");
            }
            return bhvAttr.DefaultName;
        }

        public BehaviorModelReferenceTypeConverter(Func<IEnumerable<Type>> iterator)
        {
            m_bhvModelTypeIterator = iterator;
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

                var bmRef = value as BehaviorModelReference;

                return bmRef == null || bmRef.ModelType == null
                       ? "{null}"
                       : GetBehaviorName(bmRef.ModelType);
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

                var bhvModel = m_bhvModelTypeIterator().FirstOrDefault(bm => GetBehaviorName(bm) == str);
                return bhvModel != null ? new BehaviorModelReference { ModelType = bhvModel } : null;
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
            foreach (var bm in m_bhvModelTypeIterator())
            {
                yield return GetBehaviorName(bm);
            }
        }
    }
}
