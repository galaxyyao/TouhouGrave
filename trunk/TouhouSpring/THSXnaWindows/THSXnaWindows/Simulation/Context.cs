﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Context : BaseController
    {
        private class Branch
        {
            public List<Choice> ChoicePath;
            public Game Result;
        }

        private BaseSimulator m_simulator;
        private int m_depth;
        private List<Choice> m_currentChoicePath;
        private List<List<Choice>> m_pendingChoices = new List<List<Choice>>();

        private List<Branch> m_branches = new List<Branch>();

        public Game RootGame
        {
            get; private set;
        }

        private bool ChoiceMade
        {
            get { return m_depth <= (m_currentChoicePath != null ? m_currentChoicePath.Count : 0); }
        }

        private Choice NextChoice
        {
            get { return m_currentChoicePath[m_depth - 1]; }
        }

        public Context(Game game, BaseSimulator simulator)
            : base(true)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            else if (simulator == null)
            {
                throw new ArgumentNullException("simulator");
            }

            m_simulator = simulator;
            RootGame = game;
            //RootGame = game.CloneForSimulation(controller);
        }

        public void Start()
        {
            while (true)
            {
                m_depth = 0;
                TryMoveNextChoice();

                var game = RootGame.CloneForSimulation(this);
                game.SimulateMainPhase();

                m_branches.Add(new Branch
                {
                    ChoicePath = m_currentChoicePath,
                    Result = game
                });

                if (m_pendingChoices.Count == 0)
                {
                    break;
                }
            }
        }

        private void ForkChoice(Choice choice)
        {
            List<Choice> newChoicePath = new List<Choice>();
            if (m_currentChoicePath != null)
            {
                for (int i = 0; i < m_currentChoicePath.Count; ++i)
                {
                    newChoicePath.Add(m_currentChoicePath[i]);
                }
            }
            newChoicePath.Add(choice);
            m_pendingChoices.Add(newChoicePath);
        }

        private void TryMoveNextChoice()
        {
            // discard the current choice path
            if (m_pendingChoices.Count > 0)
            {
                m_currentChoicePath = m_pendingChoices[0];
                m_pendingChoices.RemoveAt(0);
            }
            else
            {
                m_currentChoicePath = null;
            }
        }

        #region major interactions

        [Interactions.MessageHandler(typeof(Interactions.TacticalPhase))]
        private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            ++m_depth;

            if (!ChoiceMade)
            {
                foreach (var choice in m_simulator.TacticalPhase(interactionObj, this))
                {
                    ForkChoice(choice);
                }

                TryMoveNextChoice();
            }

            if (ChoiceMade)
            {
                NextChoice.Make(interactionObj);
            }

            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            m_simulator.SelectCards(interactionObj, this);
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            m_simulator.MessageBox(interactionObj, this);
            return false;
        }

        #endregion

        #region notifications

        [Interactions.MessageHandler(typeof(Interactions.NotifyCardEvent))]
        private bool OnNotified(Interactions.NotifyCardEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyGameEvent))]
        private bool OnNotified(Interactions.NotifyGameEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyPlayerEvent))]
        private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifySpellEvent))]
        private bool OnNotified(Interactions.NotifySpellEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        #endregion
    }
}