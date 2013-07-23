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
        }
    }
}
