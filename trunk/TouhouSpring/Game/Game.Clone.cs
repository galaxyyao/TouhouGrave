using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace TouhouSpring
{
    internal class CloneContext
    {
        public Game BaseGame { get; private set; }
        public Game ClonedGame { get; private set; }

        public CloneContext(Game baseGame, Game clonedGame)
        {
            BaseGame = baseGame;
            ClonedGame = clonedGame;
        }
    }

    partial class Game
    {
        private static readonly XmlSerializer RandomSerializer = XmlSerializer.FromTypes(new Type[] { typeof(Random) })[0];

        public Game CloneForSimulation(BaseController simulationController)
        {
            Game clonedGame = new Game();

            int numPlayers = m_players.Length;

            clonedGame.m_players = new Player[numPlayers];
            clonedGame.m_manaNeeded = new int[numPlayers];
            clonedGame.m_remainingManaNeeded = new bool[numPlayers];

            for (int i = 0; i < numPlayers; ++i)
            {
                clonedGame.m_manaNeeded[i] = m_manaNeeded[i];
                clonedGame.m_remainingManaNeeded[i] = m_remainingManaNeeded[i];
            }

            simulationController.Game = clonedGame;

            clonedGame.Players = clonedGame.m_players.ToIndexable();
            clonedGame.Random = CloneRandom();
            clonedGame.Controller = simulationController;

            // Clone the data structure (Behaviors will only be default-constructed)
            for (int i = 0; i < numPlayers; ++i)
            {
                clonedGame.m_players[i] = m_players[i].CloneForSimulation(clonedGame);
            }

            // Transfer behaviors to cloned ones
            for (int i = 0; i < numPlayers; ++i)
            {
                clonedGame.m_players[i].TransferCardsFrom(m_players[i]);
            }

            return clonedGame;
        }

        private Game() { }

        private Random CloneRandom()
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, Random);
                ms.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(ms) as Random;
            }
        }
    }
}
