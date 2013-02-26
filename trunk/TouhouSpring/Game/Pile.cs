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
        private List<ICardModel> m_orderedCardModels;

        /// <summary>
        /// Get the number of cards in Pile
        /// </summary>
        /// <returns>Number of cards in Pile</returns>
        public int Count
        {
            get { return m_orderedCardModels.Count; }
        }

        public bool Contains(ICardModel cardModel)
        {
            return m_orderedCardModels.Contains(cardModel);
        }

        /// <summary>
        /// Get the card at the bottom of the pile.
        /// </summary>
        public ICardModel Bottom
        {
            get
            {
                if (m_orderedCardModels.Count == 0)
                {
                    throw new InvalidOperationException("The pile is empty.");
                }

                return m_orderedCardModels[0];
            }
        }

        /// <summary>
        /// Get the card at the top of the pile.
        /// </summary>
        public ICardModel Top
        {
            get
            {
                if (m_orderedCardModels.Count == 0)
                {
                    throw new InvalidOperationException("The pile is empty.");
                }

                return m_orderedCardModels.Last();
            }
        }

        public Pile(List<ICardModel> modelList)
        {
            if (modelList == null)
            {
                throw new ArgumentNullException("modelList");
            }

            m_orderedCardModels = modelList;
        }

        /// <summary>
        /// Add one card to the bottom of the pile.
        /// </summary>
        /// <param name="card">The card to be added.</param>
        public void AddToBottom(ICardModel cardModel)
        {
            m_orderedCardModels.Insert(0, cardModel);
        }

        /// <summary>
        /// Add one card to the top of the pile.
        /// </summary>
        /// <param name="card">The card to be added.</param>
        public void AddToTop(ICardModel cardModel)
        {
            m_orderedCardModels.Add(cardModel);
        }

        /// <summary>
        /// Remove one card from the bottom of the pile.
        /// </summary>
        /// <returns>The card just removed.</returns>
        public ICardModel RemoveFromBottom()
        {
            var bottom = Bottom;
            m_orderedCardModels.RemoveAt(0);
            return bottom;
        }

        /// <summary>
        /// Remove one card from the top of the pile.
        /// </summary>
        /// <returns>The card just removed.</returns>
        public ICardModel RemoveFromTop()
        {
            var top = Top;
            m_orderedCardModels.RemoveAt(m_orderedCardModels.Count - 1);
            return top;
        }

        /// <summary>
        /// Shuffle the cards in the pile.
        /// </summary>
        public void Shuffle(Random random)
        {
            for (int i = 0; i < m_orderedCardModels.Count; ++i)
            {
                int swapIndex = random.Next(i, m_orderedCardModels.Count);
                if (swapIndex != i)
                {
                    var temp = m_orderedCardModels[i];
                    m_orderedCardModels[i] = m_orderedCardModels[swapIndex];
                    m_orderedCardModels[swapIndex] = temp;
                }
            }
        }
    }
}
