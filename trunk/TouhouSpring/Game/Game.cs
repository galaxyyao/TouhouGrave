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

        public static class CurrentCommand
        {
            public static object Result
            {
                get;
                set;
            }

            public static int ResultIndex
            {
                get;
                set;
            }

            public static int ResultIndex2
            {
                get;
                set;
            }
        }

        public BaseController Controller
        {
            get; private set;
        }

        internal Messaging.LetterBox LetterBox
        {
            get; private set;
        }

        public Game(IIndexable<GameStartupParameters> startUpParams, BaseController controller)
        {
            if (startUpParams == null)
            {
                throw new ArgumentNullException("startUpParams");
            }
            else if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (controller.Game != null)
            {
                throw new ArgumentException("The controller is already bound.", "controller");
            }

            int numPlayers = startUpParams.Count;
            if (numPlayers != 2)
            {
                //TODO: support game among more than 2 players
                throw new NotSupportedException("Battle of more than two players are not supported.");
            }

            m_players = new Player[numPlayers];
            m_resourceConditions = new PlayerResourceConditions[numPlayers];

            for (int i = 0; i < numPlayers; ++i)
            {
                if (startUpParams[i].m_profile == null)
                {
                    throw new ArgumentNullException(String.Format(CultureInfo.CurrentCulture, "parms[{0}].m_profile", i));
                }

                var deck = startUpParams[i].m_deck;
                var validationResult = deck.Validate();
                if (validationResult != Deck.ValidationResult.Okay)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        "The deck {0} is invalid: {1}", deck.Name, validationResult));
                }

                m_players[i] = new Player(startUpParams[i].m_profile.Name, this);
                m_players[i].Initialize(deck);
            }

            controller.Game = this;

            CurrentPhase = "";

            Players = m_players.ToIndexable();
            Random = (startUpParams[0].m_seed == -1) ? new Random() : new Random(startUpParams[0].m_seed);
            Controller = controller;
            LetterBox = new Messaging.LetterBox();

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

        internal Int32 GenerateNextCardGuid()
        {
            return Interlocked.Increment(ref m_nextCardGuid);
        }
    }
}
