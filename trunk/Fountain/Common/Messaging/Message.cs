using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Messaging
{
	public class Message
	{
		public string Text
		{
			get; private set;
		}

		public object Attachment
		{
			get; private set;
		}

		public Message(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			Text = text;
			Attachment = null;
		}

		public Message(string text, object attachment)
			: this(text)
		{
			Attachment = attachment;
		}

		public void SendTo(LetterBox letterBox)
		{
			if (letterBox == null)
			{
				throw new ArgumentNullException("letterBox");
			}
			letterBox.PutInto(this);
		}
	}
}
