using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace TouhouSpring
{
    /// <summary>
    /// Represent a single game involving two or more players.
    /// </summary>
    public partial class Game
    {
        private Int32 m_nextCardGuid;
        private Thread m_gameFlowThread;
        internal Dictionary<int, ZoneType> m_zoneConfig;

        /// <summary>
        /// Give a random number generator for this game.
        /// </summary>
        public Random Random
        {
            get; private set;
        }

        /// <summary>
        /// Tell the current game phase.
        /// </summary>
        public string CurrentPhase
        {
            get; internal set;
        }

        public int Round
        {
            get; private set;
        }

        public BaseController Controller
        {
            get; private set;
        }

        internal Messaging.LetterBox LetterBox
        {
            get; private set;
        }

        public Game(List<string> playerNames, BaseController controller)
        {
            if (playerNames == null)
            {
                throw new ArgumentNullException("playerNames");
            }
            else if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (controller.Game != null)
            {
                throw new ArgumentException("The controller is already bound.", "controller");
            }

            int numPlayers = playerNames.Count;
            if (numPlayers != 2)
            {
                //TODO: support game among more than 2 players
                throw new NotSupportedException("Battle of more than two players are not supported.");
            }

            m_players = new Player[numPlayers];
            for (int i = 0; i < numPlayers; ++i)
            {
                m_players[i] = new Player(playerNames[i], i, this);
            }

            controller.Game = this;

            CurrentPhase = "";

            Players = m_players.ToIndexable();
            Random = new Random(controller.GetRandomSeed());
            Controller = controller;
            LetterBox = new Messaging.LetterBox();
        }

        public void Initialize(List<Deck> playerDecks)
        {
            m_zoneConfig = new Dictionary<int, ZoneType>()
            {
                {SystemZone.Library,        ZoneType.Library},
                {SystemZone.Hand,           ZoneType.OffBattlefield},
                {SystemZone.Sacrifice,      ZoneType.OffBattlefield},
                {SystemZone.Battlefield,    ZoneType.OnBattlefield},
                {SystemZone.Graveyard,      ZoneType.Library},
                {SystemZone.Assist,         ZoneType.OffBattlefield},
            };

            for (int i = 0; i < m_players.Length; ++i)
            {
                var deck = playerDecks[i];
                var validationResult = deck.Validate();
                if (validationResult != Deck.ValidationResult.Okay)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        "The deck {0} is invalid: {1}", deck.Id, validationResult));
                }

                m_players[i].Initialize(deck);
            }
        }

        public void StartGameFlowThread()
        {
            if (m_gameFlowThread != null)
            {
                throw new InvalidOperationException("Game flow thread is running.");
            }

            // run the game flow in another thread
            m_gameFlowThread = new Thread(GameFlowMain)
            {
                IsBackground = true
            };
            m_gameFlowThread.Start();
        }

        public void OverrideController(BaseController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (Controller != null)
            {
                Controller.Game = null;
            }
            Controller = controller;
            Controller.Game = this;
        }

        public CardInstance FindCard(int guid, int zoneId, int player)
        {
            if (player < 0 || player > m_players.Length)
            {
                throw new ArgumentOutOfRangeException("player");
            }
            var zone = m_players[player].m_zones.GetZone(zoneId);
            if (zone == null || zone.CardInstances == null)
            {
                throw new ArgumentException("zone");
            }
            foreach (var card in zone.CardInstances)
            {
                if (card.Guid == guid)
                {
                    return card;
                }
            }
            return null;
        }

        internal Int32 GenerateNextCardGuid()
        {
            return Interlocked.Increment(ref m_nextCardGuid);
        }
    }
}
