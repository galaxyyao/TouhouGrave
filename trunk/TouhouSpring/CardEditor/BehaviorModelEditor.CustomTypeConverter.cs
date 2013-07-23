using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    static partial class BehaviorModelEditor
    {
        public class CustomTypeConverter : ExpandableObjectConverter
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

                    var bhvModel = value as Behaviors.IBehaviorModel;

                    return bhvModel == null
                           ? "{null}"
                           : GetBehaviorName(bhvModel.GetType());
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
