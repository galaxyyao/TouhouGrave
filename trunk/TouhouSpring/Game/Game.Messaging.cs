using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class Game
	{
		private IIndexable<Messaging.LetterBox> m_letterBoxes;

		/// <summary>
		/// The letter box object of Game flow thread for communicating with controllers
		/// </summary>
		public IIndexable<Messaging.LetterBox> LetterBoxes
		{
			get { return m_letterBoxes; }
		}

		/// <summary>
		/// Wait the next message arrive from the specified controller.
		/// </summary>
		/// <param name="controller">The controller to be waited</param>
		/// <returns>The arrived message</returns>
		internal Messaging.Message WaitMessageFrom(BaseController controller)
		{
			Messaging.LetterBox letterBox = m_letterBoxes[Array.IndexOf(m_controllers, controller)];
			return letterBox.WaitForNextMessage();
		}

		/// <summary>
		/// Wait the next message arrive from the specified controller.
		/// </summary>
		/// <param name="controller">The controller to be waited</param>
		/// <param name="text">The specific message text</param>
		/// <returns>The arrived message</returns>
		internal Messaging.Message WaitMessageFrom(BaseController controller, string text)
		{
			Messaging.Message msg = WaitMessageFrom(controller);
			if (msg.Text != text)
			{
				throw new Messaging.UnexpectedMessageException(text, msg.Text);
			}
			return msg;
		}

		/// <summary>
		/// Initialize letter box array.
		/// </summary>
		private void InitializeLetterBoxes()
		{
			Messaging.LetterBox[] letterBoxes = new Messaging.LetterBox[m_controllers.Length];
			m_controllers.Length.Repeat(i => letterBoxes[i] = new Messaging.LetterBox());
			m_letterBoxes = letterBoxes.ToIndexable();
		}
	}
}
