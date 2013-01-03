using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// Represent a single game involving two or more players.
    /// </summary>
    public partial class Game
    {
        private Random m_randomGenerator = new Random();

        /// <summary>
        /// Give a random number generator for this game.
        /// </summary>
        public Random Random
        {
            get { return m_randomGenerator; }
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

        public Game(IIndexable<GameStartupParameters> startUpParams)
        {
            if (startUpParams == null)
            {
                throw new ArgumentNullException("startUpParams");
            }

            int numPlayers = startUpParams.Count;
            if (numPlayers != 2)
            {
                //TODO: support game among more than 2 players
                throw new NotSupportedException("Battle of more than two players are not supported.");
            }

            m_profiles = new Profile[numPlayers];
            m_players = new Player[numPlayers];
            m_controllers = new BaseController[numPlayers];

            for (int i = 0; i < numPlayers; ++i)
            {
                if (startUpParams[i].m_profile == null)
                {
                    throw new ArgumentNullException(String.Format(CultureInfo.CurrentCulture, "parms[{0}].m_profile", i));
                }
                else if (startUpParams[i].m_controller == null)
                {
                    throw new ArgumentNullException(String.Format(CultureInfo.CurrentCulture, "parms[{0}].m_controller", i));
                }

                var deck = startUpParams[i].m_deck;
                var validationResult = deck.Validate();
                if (validationResult != Deck.ValidationResult.Okay)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        "The deck {0} is invalid: {1}", deck.Name, validationResult));
                }

                m_profiles[i] = startUpParams[i].m_profile;
                m_controllers[i] = startUpParams[i].m_controller;

                m_players[i] = new Player(m_profiles[i], this, m_controllers[i]);
                m_players[i].Initialize(deck);
            }

            CurrentPhase = "";

            InitializeLetterBoxes();
            StartGameFlowThread();
        }
    }
}
