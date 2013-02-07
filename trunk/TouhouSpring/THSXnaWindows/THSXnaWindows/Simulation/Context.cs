﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Context : BaseController
    {
        public class Branch
        {
            public List<Choice> ChoicePath;
            public Game Result;
        }

        private class PendingBranch
        {
            public List<Choice> ChoicePath;

            // save point
            public Game Root;
            public int Order;
            public int Depth;
        }

        private PendingBranch m_currentBranch;
        private int m_currentBranchDepth;
        private int m_currentBranchOrder;
        private List<PendingBranch> m_pendingBranches = new List<PendingBranch>();

        private BaseSimulator m_simulator;
        private List<Branch> m_branches = new List<Branch>();

        public Game RootGame
        {
            get; private set;
        }

        public IEnumerable<Branch> Branches
        {
            get { return m_branches; }
        }

        public int BranchCount
        {
            get { return m_branches.Count; }
        }

        private bool ChoiceMade
        {
            get { return m_currentBranchDepth <= (m_currentBranch.ChoicePath != null ? m_currentBranch.ChoicePath.Count : 0); }
        }

        private Choice NextChoice
        {
            get { return m_currentBranch.ChoicePath[m_currentBranchDepth - 1]; }
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
        }

        public void Start()
        {
            while (true)
            {
                TryMoveNextBranch();

                if (m_currentBranch.Root != null)
                {
                    m_currentBranchDepth = m_currentBranch.Depth;
                    m_currentBranchOrder = m_currentBranch.Order;
                    m_currentBranch.Root.RunTurnFromMainPhase();
                }
                else
                {
                    m_currentBranchDepth = 0;
                    m_currentBranchOrder = 0;
                    m_currentBranch.Root = RootGame.CloneWithController(this);
                    m_currentBranch.Root.RunTurn();
                }

                m_branches.Add(new Branch
                {
                    ChoicePath = m_currentBranch.ChoicePath,
                    Result = m_currentBranch.Root
                });

                if (m_pendingBranches.Count == 0)
                {
                    break;
                }
            }
        }

        private PendingBranch ForkBranch(Choice choice)
        {
            var newBranch = new PendingBranch { ChoicePath = new List<Choice>() };
            if (m_currentBranch.ChoicePath != null)
            {
                for (int i = 0; i < m_currentBranch.ChoicePath.Count; ++i)
                {
                    newBranch.ChoicePath.Add(m_currentBranch.ChoicePath[i]);
                }
            }
            newBranch.ChoicePath.Add(choice);
            m_pendingBranches.Insert(0, newBranch);
            return newBranch;
        }

        private void TryMoveNextBranch()
        {
            // discard the current choice path
            if (m_pendingBranches.Count > 0)
            {
                m_currentBranch = m_pendingBranches[0];
                m_pendingBranches.RemoveAt(0);
            }
            else
            {
                m_currentBranch = new PendingBranch();
            }
        }

        private void OnInteraction(Interactions.BaseInteraction io, IEnumerable<Choice> choices)
        {
            ++m_currentBranchDepth;

            if (!ChoiceMade)
            {
                bool isTacticalPhase = io is Interactions.TacticalPhase;

                foreach (var choice in choices)
                {
                    var branch = ForkBranch(choice);

                    if (isTacticalPhase)
                    {
                        // create save point
                        branch.Root = io.Game.CloneWithController(this);
                        branch.Depth = m_currentBranchDepth - 1;
                        branch.Order = m_currentBranchOrder;
                    }
                }

                TryMoveNextBranch();
            }

            if (ChoiceMade)
            {
                var nextChoice = NextChoice;
                nextChoice.Make(io);
                m_currentBranchOrder = Math.Max(nextChoice.Order, m_currentBranchOrder);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region major interactions

        [Interactions.MessageHandler(typeof(Interactions.TacticalPhase))]
        private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.TacticalPhase(interactionObj, m_currentBranchOrder));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.SelectCards(interactionObj, m_currentBranchOrder));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.MessageBox(interactionObj, m_currentBranchOrder));
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
