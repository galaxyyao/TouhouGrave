using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
	public partial class BaseController
	{
		private Messaging.LetterBox m_letterBox = new Messaging.LetterBox();
		private Interactions.MessageMap m_messageMap;

		/// <summary>
		/// The letter box object of this controller for communicating with Game
		/// </summary>
		public Messaging.LetterBox Inbox
		{
			get { return m_letterBox; }
		}

		public Messaging.LetterBox Outbox
		{
			get { return Game.LetterBoxes[Game.Players.IndexOf(Player)]; }
		}

		/// <summary>
		/// The frontend calls this method in client thread to process arrived messages.
		/// </summary>
		/// <returns>True if further interaction is needed; false otherwise</returns>
		public bool ProcessMessage()
		{
			return m_messageMap.Process(m_letterBox);
		}

		/// <summary>
		/// Construct a message text-handler map for message handling.
		/// </summary>
		private void InitializeMessaging()
		{
			if (m_messageMap != null)
			{
				throw new InvalidOperationException("Message map already constructed.");
			}

			m_messageMap = new Interactions.MessageMap(this);
		}
	}
}
