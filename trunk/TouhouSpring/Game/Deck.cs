using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// A preselected set of cards.
    /// </summary>
    public class Deck : IIndexable<ICardModel>
    {
		private string m_name;
        private List<ICardModel> m_cards = new List<ICardModel>();

        public ICardModel this[int index]
		{
			get { return m_cards[index]; }
		}

		public string Name
		{
			get { return m_name; }
		}

		public int Count
		{
			get { return m_cards.Count; }
		}

        public int IndexOf(ICardModel element)
		{
			return m_cards.IndexOf(element);
		}

		public Deck(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			m_name = name;
		}

        public void Add(ICardModel cardModel)
		{
			if (cardModel == null)
			{
				throw new ArgumentNullException("cardModel");
			}
			m_cards.Add(cardModel);
		}

        public IEnumerator<ICardModel> GetEnumerator()
		{
			return m_cards.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_cards.GetEnumerator();
		}
    }
}
