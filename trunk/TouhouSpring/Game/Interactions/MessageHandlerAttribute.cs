using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MessageHandlerAttribute : Attribute
    {
        public Type InteractionType
        {
            get; private set;
        }

        public MessageHandlerAttribute(Type interactionType)
        {
            if (interactionType == null)
            {
                throw new ArgumentNullException("interactionType");
            }

            InteractionType = interactionType;
        }
    }
}
