using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    static partial class BehaviorModelReferenceEditor
    {
        public class CustomPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor m_pd;

            public CustomPropertyDescriptor(PropertyDescriptor pd)
                : base(pd)
            {
                m_pd = pd;
            }

            public override Type ComponentType
            {
                get { return m_pd.ComponentType; }
            }

            public override bool IsReadOnly
            {
                get { return m_pd.IsReadOnly; }
            }

            public override Type PropertyType
            {
                get { return m_pd.PropertyType; }
            }

            public override bool CanResetValue(object component)
            {
                return m_pd.CanResetValue((component as BehaviorModelReference).Value);
            }

            public override object GetValue(object component)
            {
                return m_pd.GetValue((component as BehaviorModelReference).Value);
            }

            public override void ResetValue(object component)
            {
                m_pd.ResetValue((component as BehaviorModelReference).Value);
            }

            public override void SetValue(object component, object value)
            {
                m_pd.SetValue((component as BehaviorModelReference).Value, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return m_pd.ShouldSerializeValue((component as BehaviorModelReference).Value);
            }
        }
    }
}
