using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public class GameStartupParameters
	{
		public Profile m_profile;
		public Deck m_deck;
        public ICardModel m_hero;
		public BaseController m_controller;
	}
}
