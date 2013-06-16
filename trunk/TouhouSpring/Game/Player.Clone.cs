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
            var clonedPlayer = new Player(Name, Index, game);

            clonedPlayer.Health = Health;
            clonedPlayer.Mana = Mana;

            foreach (var mod in m_manaAddModifiers)
            {
                clonedPlayer.m_manaAddModifiers.Add(mod);
            }

            foreach (var mod in m_manaSubtractModifiers)
            {
                clonedPlayer.m_manaSubtractModifiers.Add(mod);
            }

            foreach (var mod in m_lifeAddModifiers)
            {
                clonedPlayer.m_lifeAddModifiers.Add(mod);
            }

            foreach (var mod in m_lifeSubtractModifiers)
            {
                clonedPlayer.m_lifeSubtractModifiers.Add(mod);
            }

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
                var clonedCard = m_battlefieldCards[i].Clone(clonedPlayer);
                clonedPlayer.m_battlefieldCards.Add(clonedCard);
                clonedPlayer.Game.SubscribeCardToCommands(clonedCard);
            }

            if (Hero != null)
            {
                throw new NotImplementedException("Hero clone not implemented");
            }

            for (int i = 0; i < m_assists.Count; ++i)
            {
                clonedPlayer.m_assists.Add(m_assists[i].Clone(clonedPlayer));
            }

            for (int i = 0; i < m_activatedAssists.Count; ++i)
            {
                clonedPlayer.m_activatedAssists.Add(clonedPlayer.m_assists[m_assists.IndexOf(m_activatedAssists[i])]);
                clonedPlayer.Game.SubscribeCardToCommands(clonedPlayer.m_activatedAssists[i]);
            }

            for (int i = 0; i < m_library.Count; ++i)
            {
                clonedPlayer.m_library.Add(m_library[i]);
            }

            for (int i = 0; i < m_graveyard.Count; ++i)
            {
                clonedPlayer.m_graveyard.Add(m_graveyard[i]);
            }

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
        }
    }
}
