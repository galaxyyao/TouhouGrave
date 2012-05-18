using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	/// <summary>
	/// Represents a group of cards piling up together.
	/// </summary>
    public class Pile
    {
		// list start from bottom to top
		private List<BaseCard> m_orderedCards = new List<BaseCard>();

		/// <summary>
		/// Get the card at the bottom of the pile.
		/// </summary>
		public BaseCard Bottom
		{
			get
			{
				if (m_orderedCards.Count == 0)
				{
					throw new InvalidOperationException("The pile is empty.");
				}

				return m_orderedCards[0];
			}
		}

		/// <summary>
		/// Get the card at the top of the pile.
		/// </summary>
		public BaseCard Top
		{
			get
			{
				if (m_orderedCards.Count == 0)
				{
					throw new InvalidOperationException("The pile is empty.");
				}

				return m_orderedCards[m_orderedCards.Count - 1];
			}
		}

		/// <summary>
		/// Add one card to the bottom of the pile.
		/// </summary>
		/// <param name="card">The card to be added.</param>
		public void AddCardToBottom(BaseCard card)
		{
			m_orderedCards.Insert(0, card);
		}

		/// <summary>
		/// Add one card to the top of the pile.
		/// </summary>
		/// <param name="card">The card to be added.</param>
		public void AddCardToTop(BaseCard card)
		{
			m_orderedCards.Add(card);
		}

		/// <summary>
		/// Remove one card from the bottom of the pile.
		/// </summary>
		/// <returns>The card just removed.</returns>
		public BaseCard RemoveCardFromBottom()
		{
			BaseCard bottom = Bottom;
			m_orderedCards.RemoveAt(0);
			return bottom;
		}

		/// <summary>
		/// Remove one card from the top of the pile.
		/// </summary>
		/// <returns>The card just removed.</returns>
		public BaseCard RemoveCardFromTop()
		{
			BaseCard top = Top;
			m_orderedCards.RemoveAt(m_orderedCards.Count - 1);
			return top;
		}

		/// <summary>
		/// Shuffle the cards in the pile.
		/// </summary>
        public void Shuffle(Random random)
		{
			for (int i = 0; i < m_orderedCards.Count; ++i)
			{
				int swapIndex = random.Next(i, m_orderedCards.Count);
				if (swapIndex != i)
				{
					BaseCard temp = m_orderedCards[i];
					m_orderedCards[i] = m_orderedCards[swapIndex];
					m_orderedCards[swapIndex] = temp;
				}
			}
        }

		/// <summary>
		/// Query cards from the pile meeting the given predication.
		/// </summary>
		/// <param name="p">The predication.</param>
		/// <returns>A collection of the queried cards.</returns>
		public BaseCard[] QueryCards(Predicate<BaseCard> p)
        {
            return (from card in m_orderedCards where p(card) select card).ToArray();
        }

		/// <summary>
		/// Get the number of cards in Pile
		/// </summary>
		/// <returns>Number of cards in Pile</returns>
		public int Count()
		{
			return m_orderedCards.Count;
		}
    }
}
