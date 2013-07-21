using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    static partial class BehaviorModelReferenceEditor
    {
        public class CustomTypeConverter : TypeConverter
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

            public CustomTypeConverter(Func<IEnumerable<Type>> iterator)
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

                    return bmRef == null || bmRef.Value == null
                           ? "{null}"
                           : GetBehaviorName(bmRef.Value.GetType());
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
                    return bhvModel != null ? new BehaviorModelReference { Value = bhvModel.Assembly.CreateInstance(bhvModel.FullName) as Behaviors.IBehaviorModel } : null;
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

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                var bhvModelRef = value as BehaviorModelReference;
                if (bhvModelRef != null && bhvModelRef.Value != null)
                {
                    var bhvModelProps = TypeDescriptor.GetProperties(bhvModelRef.Value);
                    var pds = new PropertyDescriptor[bhvModelProps.Count];
                    for (int i = 0; i < pds.Length; ++i)
                    {
                        pds[i] = new CustomPropertyDescriptor(bhvModelProps[i]);
                    }
                    return new PropertyDescriptorCollection(pds);
                }
                return base.GetProperties(context, value, attributes);
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
}
