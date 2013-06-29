using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
#if WINDOWS
    using System.ComponentModel;

    [TypeConverter(typeof(BehaviorModelReference.ProxiedTypeConverter))]
#endif
    public class BehaviorModelReference
    {
#if WINDOWS
        public static TypeConverter TypeConverter
        {
            get; set;
        }

        internal class ProxiedTypeConverter : ProxiedCustomTypeConverter
        {
            protected override TypeConverter GetProxy()
            {
                return BehaviorModelReference.TypeConverter;
            }
        }
#endif

        public Type ModelType
        {
            get; set;
        }

        public Behaviors.IBehaviorModel Instantiate()
        {
            return ModelType.Assembly.CreateInstance(ModelType.FullName) as Behaviors.IBehaviorModel;
        }
    }
}
