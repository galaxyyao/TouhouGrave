using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseController
    {
        private Messaging.LetterBox m_letterBox;
        private Interactions.MessageMap m_messageMap;

        /// <summary>
        /// The letter box object of this controller for communicating with Game
        /// </summary>
        internal Messaging.LetterBox Inbox
        {
            get { return m_letterBox; }
        }

        /// <summary>
        /// The frontend calls this method in client thread to process arrived messages.
        /// </summary>
        /// <returns>True if further interaction is needed; false otherwise</returns>
        public bool ProcessMessage()
        {
            if (m_letterBox == null)
            {
                throw new InvalidOperationException("This method can only be invoked in non-sync mode.");
            }
            return m_messageMap.Process(m_letterBox.Receive());
        }

        internal bool ProcessMessage(Messaging.Message msg)
        {
            if (m_letterBox != null)
            {
                throw new InvalidOperationException("This method can only be invoked in sync mode.");
            }
            return m_messageMap.Process(msg);
        }

        /// <summary>
        /// Construct a message text-handler map for message handling.
        /// </summary>
        private void InitializeMessaging(bool syncMode)
        {
            if (m_messageMap != null)
            {
                throw new InvalidOperationException("Message map already constructed.");
            }

            m_letterBox = syncMode ? null : new Messaging.LetterBox();
            m_messageMap = new Interactions.MessageMap(this);
        }
    }
}
