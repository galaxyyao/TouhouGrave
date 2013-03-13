using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Network
{
    public class MessageQueue
    {
        protected List<string> m_messageQueue = new List<string>();
        protected Client m_client = null;

        public bool IsEmpty
        {
            get
            {
                return (m_messageQueue.Count == 0);
            }
        }

        public MessageQueue(Client client)
        {
            m_client = client;
        }

        public void Queue(string message)
        {
            m_messageQueue.Add(message);
        }

        public virtual void Flush()
        {
        }

        public void Clear()
        {
            m_messageQueue.Clear();
        }
    }
}
