using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Messaging
{
	public class LetterBox
	{
		private Message m_message;
		private Threading.IEvent m_event = Threading.Factory.Instance.NewEvent(true, false);
		private Threading.IMutex m_mutex = Threading.Factory.Instance.NewMutex();

		public Message Receive()
		{
			if (m_message == null)
			{
				return null;
			}

			m_mutex.Lock();
			try
			{
				Message msg = m_message;
				m_message = null;
				m_event.Reset();
				return msg;
			}
			finally
			{
				m_mutex.Unlock();
			}
		}

		public Message WaitForNextMessage()
		{
			m_event.Wait();
			return Receive();
		}

		internal void PutInto(Message message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			m_mutex.Lock();
			try
			{
				if (m_message != null)
				{
					throw new LetterBoxIsFullException();
				}
				m_message = message;
				m_event.Singal();
			}
			finally
			{
				m_mutex.Unlock();
			}
		}
	}
}
