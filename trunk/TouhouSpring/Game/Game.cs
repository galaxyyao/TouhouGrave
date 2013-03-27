﻿using System;
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
            get;
            private set;
        }

        /// <summary>
        /// Tell the current game phase.
        /// </summary>
        public string CurrentPhase
        {
            get;
            internal set;
        }

        public int Round
        {
            get;
            private set;
        }

        public BaseController Controller
        {
            get;
            private set;
        }

        internal Messaging.LetterBox LetterBox
        {
            get;
            private set;
        }

        public Game(List<Deck> playerDecks, List<string> playerIds, BaseController controller, int seed)
        {
            if (playerDecks == null)
            {
                throw new ArgumentNullException("decks");
            }
            else if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (controller.Game != null)
            {
                throw new ArgumentException("The controller is already bound.", "controller");
            }

            int numPlayers = playerDecks.Count;
            if (numPlayers != 2)
            {
                //TODO: support game among more than 2 players
                throw new NotSupportedException("Battle of more than two players are not supported.");
            }

            m_players = new Player[numPlayers];

            for (int i = 0; i < numPlayers; ++i)
            {
                var deck = playerDecks[i];
                var validationResult = deck.Validate();
                if (validationResult != Deck.ValidationResult.Okay)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        "The deck {0} is invalid: {1}", deck.Name, validationResult));
                }

                m_players[i] = new Player(playerIds[i], i, this);
                m_players[i].Initialize(deck);
            }

            controller.Game = this;

            CurrentPhase = "";

            Players = m_players.ToIndexable();
            Random = (seed == -1) ? new Random() : new Random(seed);
            Controller = controller;
            LetterBox = new Messaging.LetterBox();
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

        internal Int32 GenerateNextCardGuid()
        {
            return Interlocked.Increment(ref m_nextCardGuid);
        }
    }
}
