﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class BaseInteraction
    {
        // This member is only used when the interaction is run in sync mode
        private Messaging.Message m_syncModeMessage;

        public enum PlayerAction
        {
            PlayCard,       // play a card (hero, warrior or spell card)
            ActivateAssist, // activate an assist
            CastSpell,      // cast a spell from a warrior on battlefield
            Sacrifice,      // put one hand card to sacrifice zone
            Redeem,         // return one card from sacrifice to hand
            AttackCard,     // card attacks a opponent card
            AttackPlayer,   // card attacks the opponent player
            Pass,
            SelectCards,
            SelectNumber,
            MessageBox,
            Abort
        }

        public Game Game
        {
            get;
            private set;
        }

        internal static string GetMessageText(Type interactionType)
        {
            if (interactionType == null)
            {
                throw new ArgumentNullException("interactionType");
            }
            return "interaction:" + interactionType.FullName;
        }

        protected BaseInteraction(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            Game = game;
        }

        /// <summary>
        /// Called by Game object in game flow thread to notify a controller of the happening
        /// of this interaction process.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting attachment</typeparam>
        /// <returns>The resulting attachment</returns>
        protected TResult NotifyAndWait<TResult>()
        {
            string msgText = GetMessageText(GetType());
            var outboxMsg = new Messaging.Message(msgText, this);
            Messaging.Message inboxMsg;

            if (Game.LetterBox != null && Game.Controller.Inbox != null)
            {
                outboxMsg.SendTo(Game.Controller.Inbox);
                inboxMsg = Game.LetterBox.WaitForNextMessage();
            }
            else
            {
                // run interaction without inter-thread communication (sync mode)
                Game.Controller.ProcessMessage(outboxMsg);
                inboxMsg = m_syncModeMessage;
                m_syncModeMessage = null;
            }

            Debug.Assert(inboxMsg != null);

            if (inboxMsg.Text != msgText)
            {
                throw new Messaging.UnexpectedMessageException(msgText, inboxMsg.Text);
            }

            return (TResult)inboxMsg.Attachment;
        }

        /// <summary>
        /// Called by the interaction object itself in client thread to send the result back to
        /// the game flow thread.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting attachment</typeparam>
        /// <param name="result">The result of this interaction</param>
        protected void RespondBack<TResult>(TResult result)
        {
            Game.Controller.OnRespondBack(this, result);

            string msgText = GetMessageText(GetType());
            var msg = new Messaging.Message(msgText, result);

            if (Game.LetterBox != null)
            {
                msg.SendTo(Game.LetterBox);
            }
            else
            {
                m_syncModeMessage = msg;
            }
        }
    }

    [Serializable]
    public class InteractionValidationFailException : Exception
    {
        public InteractionValidationFailException()
        { }

        public InteractionValidationFailException(string message)
            : base(message)
        { }

        public InteractionValidationFailException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected InteractionValidationFailException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
