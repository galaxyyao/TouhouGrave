using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring
{
    public partial class BaseController
    {
        private static readonly string MsgTacticalPhase     = BaseInteraction.GetMessageText(typeof(TacticalPhase));
        private static readonly string MsgSelectCards       = BaseInteraction.GetMessageText(typeof(SelectCards));
        private static readonly string MsgMessageBox        = BaseInteraction.GetMessageText(typeof(MessageBox));
        private static readonly string MsgSelectNumber      = BaseInteraction.GetMessageText(typeof(SelectNumber));
        private static readonly string MsgNotifyCardEvent   = BaseInteraction.GetMessageText(typeof(NotifyCardEvent));
        private static readonly string MsgNotifyGameEvent   = BaseInteraction.GetMessageText(typeof(NotifyGameEvent));
        private static readonly string MsgNotifyPlayerEvent = BaseInteraction.GetMessageText(typeof(NotifyPlayerEvent));
        private static readonly string MsgNotifySpellEvent  = BaseInteraction.GetMessageText(typeof(NotifySpellEvent));

        private Messaging.LetterBox m_letterBox;

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
            return DispatchMessage(m_letterBox.Receive());
        }

        internal bool ProcessMessage(Messaging.Message msg)
        {
            if (m_letterBox != null)
            {
                throw new InvalidOperationException("This method can only be invoked in sync mode.");
            }
            return DispatchMessage(msg);
        }

        protected virtual bool OnTacticalPhase(TacticalPhase io) { return false; }
        protected virtual bool OnSelectCards(SelectCards io) { return false; }
        protected virtual bool OnMessageBox(MessageBox io) { return false; }
        protected virtual bool OnSelectNumber(SelectNumber io) { return false; }
        protected virtual bool OnNotified(NotifyCardEvent io) { return false; }
        protected virtual bool OnNotified(NotifyGameEvent io) { return false; }
        protected virtual bool OnNotified(NotifyPlayerEvent io) { return false; }
        protected virtual bool OnNotified(NotifySpellEvent io) { return false; }

        /// <summary>
        /// Construct a message text-handler map for message handling.
        /// </summary>
        private void InitializeMessaging(bool syncMode)
        {
            m_letterBox = syncMode ? null : new Messaging.LetterBox();
        }

        private bool DispatchMessage(Messaging.Message message)
        {
            if (message != null)
            {
                // TODO: if there is no message other than IO messages, maybe we could use a better message id than
                // a text
                if (message.Text == MsgTacticalPhase)
                {
                    return OnTacticalPhase(message.Attachment as TacticalPhase);
                }
                else if (message.Text == MsgSelectCards)
                {
                    return OnSelectCards(message.Attachment as SelectCards);
                }
                else if (message.Text == MsgMessageBox)
                {
                    return OnMessageBox(message.Attachment as MessageBox);
                }
                else if (message.Text == MsgSelectNumber)
                {
                    return OnSelectNumber(message.Attachment as SelectNumber);
                }
                else if (message.Text == MsgNotifyCardEvent)
                {
                    return OnNotified(message.Attachment as NotifyCardEvent);
                }
                else if (message.Text == MsgNotifyGameEvent)
                {
                    return OnNotified(message.Attachment as NotifyGameEvent);
                }
                else if (message.Text == MsgNotifyPlayerEvent)
                {
                    return OnNotified(message.Attachment as NotifyPlayerEvent);
                }
                else if (message.Text == MsgNotifySpellEvent)
                {
                    return OnNotified(message.Attachment as NotifySpellEvent);
                }
                else
                {
                    throw new NotImplementedException(String.Format("Undefined message {0}", message.Text));
                }
            }

            return false;
        }
    }
}
