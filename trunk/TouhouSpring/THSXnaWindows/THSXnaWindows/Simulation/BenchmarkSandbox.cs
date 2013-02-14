using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class BenchmarkSandbox : BaseController, ISandbox, IContext
    {
        private class PendingBranch
        {
            public Choice[] ChoicePath;

            // save point
            public Game Root;
            public int Order;
            public int Depth;
        }

        private PendingBranch m_currentBranch;
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

        public int CurrentBranchDepth
        {
            get; private set;
        }

        public int CurrentBranchOrder
        {
            get; private set;
        }

        public Choice[] CurrentBranchChoicePath
        {
            get { return m_currentBranch.ChoicePath; }
        }

        private bool ChoiceMade
        {
            get { return CurrentBranchDepth <= m_currentBranch.ChoicePath.Length; }
        }

        private Choice NextChoice
        {
            get { return m_currentBranch.ChoicePath[CurrentBranchDepth - 1]; }
        }

        public BenchmarkSandbox(Game game, BaseSimulator simulator)
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

        public void Run()
        {
            m_pendingBranches.Add(new PendingBranch { ChoicePath = new Choice[0] });

            while (TryMoveNextBranch())
            {
                if (m_currentBranch.Root != null)
                {
                    CurrentBranchDepth = m_currentBranch.Depth;
                    CurrentBranchOrder = m_currentBranch.Order;
                    m_currentBranch.Root.RunTurnFromMainPhase();
                }
                else
                {
                    CurrentBranchDepth = 0;
                    CurrentBranchOrder = 0;
                    m_currentBranch.Root = RootGame.CloneWithController(this);
                    m_currentBranch.Root.RunTurn();
                }

                m_branches.Add(new Branch
                {
                    ChoicePath = m_currentBranch.ChoicePath,
                    Result = m_currentBranch.Root
                });
            }
        }

        private PendingBranch ForkBranch(Choice choice)
        {
            var newChoicePath = new Choice[CurrentBranchChoicePath.Length + 1];
            Array.Copy(CurrentBranchChoicePath, newChoicePath, CurrentBranchChoicePath.Length);
            newChoicePath[CurrentBranchChoicePath.Length] = choice;
            var newBranch = new PendingBranch { ChoicePath = newChoicePath };
            m_pendingBranches.Insert(0, newBranch);
            return newBranch;
        }

        private bool TryMoveNextBranch()
        {
            // discard the current choice path
            if (m_pendingBranches.Count > 0)
            {
                m_currentBranch = m_pendingBranches[0];
                m_pendingBranches.RemoveAt(0);
                return true;
            }

            return false;
        }

        private void OnInteraction(Interactions.BaseInteraction io, IEnumerable<Choice> choices)
        {
            ++CurrentBranchDepth;

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
                        branch.Depth = CurrentBranchDepth - 1;
                        branch.Order = CurrentBranchOrder;
                    }
                }

                TryMoveNextBranch();
            }

            if (ChoiceMade)
            {
                var nextChoice = NextChoice;
                nextChoice.Make(io);
                CurrentBranchOrder = Math.Max(nextChoice.Order, CurrentBranchOrder);
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
            OnInteraction(interactionObj, m_simulator.TacticalPhase(interactionObj, this));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.SelectCards(interactionObj, this));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.MessageBox(interactionObj, this));
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
