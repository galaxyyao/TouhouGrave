using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Player
    {
        internal Player Clone(Game game)
        {
            var clonedPlayer = new Player(Name, game);

            clonedPlayer.Health = Health;
            clonedPlayer.Mana = Mana;

            for (int i = 0; i < m_handSet.Count; ++i)
            {
                clonedPlayer.m_handSet.Add(m_handSet[i].Clone(clonedPlayer));
            }

            for (int i = 0; i < m_sacrifices.Count; ++i)
            {
                clonedPlayer.m_sacrifices.Add(m_sacrifices[i].Clone(clonedPlayer));
            }

            for (int i = 0; i < m_battlefieldCards.Count; ++i)
            {
                clonedPlayer.m_battlefieldCards.Add(m_battlefieldCards[i].Clone(clonedPlayer));
            }

            if (Hero != null)
            {
                throw new NotImplementedException("Hero clone not implemented");
            }

            for (int i = 0; i < m_assists.Count; ++i)
            {
                clonedPlayer.m_assists.Add(m_assists[i].Clone(clonedPlayer));
            }
            clonedPlayer.ActivatedAssist = ActivatedAssist != null ? clonedPlayer.m_assists[m_assists.IndexOf(ActivatedAssist)] : null;

            for (int i = 0; i < m_library.Count; ++i)
            {
                clonedPlayer.m_library.Add(m_library[i].Clone(clonedPlayer));
            }

            // skip cloning graveyard

            return clonedPlayer;
        }

        internal void TransferCardsFrom(Player original)
        {
            for (int i = 0; i < m_handSet.Count; ++i)
            {
                m_handSet[i].TransferFrom(original.m_handSet[i]);
            }

            for (int i = 0; i < m_sacrifices.Count; ++i)
            {
                m_sacrifices[i].TransferFrom(original.m_sacrifices[i]);
            }

            for (int i = 0; i < m_battlefieldCards.Count; ++i)
            {
                m_battlefieldCards[i].TransferFrom(original.m_battlefieldCards[i]);
            }

            if (Hero != null)
            {
                throw new NotImplementedException("Hero clone not implemented");
            }

            for (int i = 0; i < m_assists.Count; ++i)
            {
                m_assists[i].TransferFrom(original.m_assists[i]);
            }

            for (int i = 0; i < m_library.Count; ++i)
            {
                m_library[i].TransferFrom(original.m_library[i]);
            }

            // skip transferring graveyard
        }
    }
}
