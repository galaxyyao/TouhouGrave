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
		public static string GetMessageText(Type interactionType)
		{
			if (interactionType == null)
			{
				throw new ArgumentNullException("interactionType");
			}
			return "interaction:" + interactionType.FullName;
		}

		/// <summary>
		/// Called by Game object in game flow thread to notify a controller of the happening
		/// of this interaction process.
		/// </summary>
		/// <typeparam name="TResult">The type of the resulting attachment</typeparam>
		/// <param name="controller">The target controller object</param>
		/// <returns>The resulting attachment</returns>
		protected TResult NotifyAndWait<TResult>(BaseController controller)
		{
			string msgText = GetMessageText(GetType());

			new Messaging.Message(msgText, this).SendTo(controller.Inbox);
			var msg = controller.Outbox.WaitForNextMessage();
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
		/// <param name="controller">The target controller object</param>
		/// <param name="result">The result of this interaction</param>
		protected void RespondBack<TResult>(BaseController controller, TResult result)
		{
			string msgText = GetMessageText(GetType());

			new Messaging.Message(msgText, result).SendTo(controller.Outbox);
		}
	}
}
