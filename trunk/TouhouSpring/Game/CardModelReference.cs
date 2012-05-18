using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
#if WINDOWS
    using System.ComponentModel;

    [TypeConverter(typeof(CardModelReference.ProxiedTypeConverter))]
#endif
    public class CardModelReference
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
                return CardModelReference.TypeConverter;
            }
        }
#endif

        public ICardModel Target
        {
            get; set;
        }
    }
}
