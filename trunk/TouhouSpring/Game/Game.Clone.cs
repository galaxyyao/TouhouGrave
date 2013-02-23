using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class Game
    {
        public Game Clone()
        {
            return CloneWithController(null);
        }

        public Game CloneWithController(BaseController controller)
        {
            Game clonedGame = new Game();

            int numPlayers = m_players.Length;

            clonedGame.m_players = new Player[numPlayers];

            clonedGame.Players = clonedGame.m_players.ToIndexable();
            clonedGame.Random = new Random(Random);
            clonedGame.m_nextCardGuid = m_nextCardGuid;

            if (controller != null)
            {
                controller.Game = clonedGame;
                clonedGame.Controller = controller;
            }

            clonedGame.CurrentPhase = CurrentPhase;
            clonedGame.Round = Round;
            clonedGame.m_actingPlayer = m_actingPlayer;
            clonedGame.DidSacrifice = DidSacrifice;
            clonedGame.DidRedeem = DidRedeem;

            // Clone the data structure (Behaviors will only be default-constructed)
            for (int i = 0; i < numPlayers; ++i)
            {
                clonedGame.m_players[i] = m_players[i].Clone(clonedGame);
            }

            // Transfer behaviors to cloned ones
            for (int i = 0; i < numPlayers; ++i)
            {
                clonedGame.m_players[i].TransferCardsFrom(m_players[i]);
            }

            return clonedGame;
        }

        private Game() { }
    }
}
