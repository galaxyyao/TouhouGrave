using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class MessageMap
	{
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		public class HandlerAttribute : Attribute
		{
			public Type InteractionType
			{
				get;
				private set;
			}

			public HandlerAttribute(Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}

				InteractionType = type;
			}
		}

		private Dictionary<string, Func<object, bool>> m_messageMap = new Dictionary<string, Func<object, bool>>();

		public MessageMap(object controller)
		{
			foreach (var method in controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				var attribs = method.GetCustomAttributes(typeof(HandlerAttribute), true);
				if (attribs.Length == 0)
				{
					continue;
				}
				var attr = (HandlerAttribute)attribs[0];

				Type interactionType = attr.InteractionType;
				Debug.Assert(interactionType != null);
				string msgText = BaseInteraction.GetMessageText(interactionType);

				if (m_messageMap.ContainsKey(msgText))
				{
					throw new InvalidDataException(String.Format("Interaction type {0} is handled more than once.", interactionType.FullName));
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

				var methodLocal = method;
				m_messageMap.Add(msgText, delegate(object interactionObj)
				{
					object retVal = methodLocal.Invoke(controller,
													   methodLocal.GetParameters().Length == 0
													   ? new object[] { }
													   : new object[] { interactionObj });
					return methodLocal.ReturnType == typeof(bool) ? (bool)retVal : false;
				});
			}
		}

		public bool Process(Messaging.LetterBox letterBox)
		{
			if (letterBox == null)
			{
				throw new ArgumentNullException("letterBox");
			}

			Messaging.Message msg = letterBox.Receive();
			if (msg != null)
			{
				Func<object, bool> handler;
				if (m_messageMap.TryGetValue(msg.Text, out handler))
				{
					return handler(msg.Attachment);
				}
			}

			return false;
		}
	}
}
