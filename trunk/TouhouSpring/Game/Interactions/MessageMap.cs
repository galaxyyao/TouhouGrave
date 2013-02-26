using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring.Interactions
{
    class MessageMap
    {
        private static Dictionary<Type, Dictionary<string, MethodInfo>> s_messageMaps
            = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        private object m_dispatcher;
        private Dictionary<string, MethodInfo> m_messageMap;

        public MessageMap(object dispatcher)
        {
            m_dispatcher = dispatcher;

            var dispatcherType = dispatcher.GetType();
            if (!s_messageMaps.TryGetValue(dispatcherType, out m_messageMap))
            {
                lock (s_messageMaps)
                {
                    if (!s_messageMaps.TryGetValue(dispatcherType, out m_messageMap))
                    {
                        m_messageMap = new Dictionary<string, MethodInfo>();

                        foreach (var method in dispatcherType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            var attribs = method.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
                            if (attribs.Length == 0)
                            {
                                continue;
                            }
                            var attr = (MessageHandlerAttribute)attribs[0];

                            Type interactionType = attr.InteractionType;
                            Debug.Assert(interactionType != null);
                            string msgText = BaseInteraction.GetMessageText(interactionType);

                            if (m_messageMap.ContainsKey(msgText))
                            {
                                throw new InvalidDataException(String.Format(CultureInfo.CurrentCulture, "Interaction type {0} is handled more than once.", interactionType.FullName));
                            }
                            else if (method.GetParameters().Length > 1)
                            {
                                throw new InvalidDataException("Message handler shall only receive one or zero argument.");
                            }
                            else if (method.GetParameters()[0].ParameterType != interactionType)
                            {
                                throw new InvalidDataException("Message handler shall receive an argument with the same type of the interaction object.");
                            }
                            else if (method.ReturnType != typeof(void) && method.ReturnType != typeof(bool))
                            {
                                throw new InvalidDataException("Message handler shall return nothing or a boolean.");
                            }

                            m_messageMap.Add(msgText, method);
                        }

                        s_messageMaps.Add(dispatcherType, m_messageMap);
                    }
                }
            }
        }

        public bool Process(Messaging.Message msg)
        {
            if (msg != null)
            {
                MethodInfo method;
                if (m_messageMap.TryGetValue(msg.Text, out method))
                {
                    object retVal = method.Invoke(m_dispatcher,
                        method.GetParameters().Length == 0
                        ? new object[] { }
                        : new object[] { msg.Attachment });
                    return method.ReturnType == typeof(bool) ? (bool)retVal : false;
                }
            }

            return false;
        }
    }
}
