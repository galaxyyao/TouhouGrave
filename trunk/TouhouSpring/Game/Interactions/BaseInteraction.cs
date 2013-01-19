using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class BaseInteraction
    {
        public Game Game
        {
            get; private set;
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

            new Messaging.Message(msgText, this).SendTo(Game.Controller.Inbox);
            var msg = Game.LetterBox.WaitForNextMessage();
            Debug.Assert(msg != null);

            if (msg.Text != msgText)
            {
                throw new Messaging.UnexpectedMessageException(msgText, msg.Text);
            }
            return (TResult)msg.Attachment;
        }

        /// <summary>
        /// Called by the interaction object itself in client thread to send the result back to
        /// the game flow thread.
        /// </summary>
        /// <typeparam name="TResult">The type of the resulting attachment</typeparam>
        /// <param name="result">The result of this interaction</param>
        protected void RespondBack<TResult>(TResult result)
        {
            string msgText = GetMessageText(GetType());

            new Messaging.Message(msgText, result).SendTo(Game.LetterBox);
        }
    }
}
