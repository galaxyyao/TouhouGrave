using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    partial class ParallelSandbox
    {
        protected class PendingBranch
        {
            public Choice[] ChoicePath;

            // save point
            public Game Root;
            public int Order;
            public int Depth;
        }

        protected class Task : BaseController, IContext
        {
            private ParallelSandbox m_sandbox;
            private PendingBranch m_currentBranch;

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

            public Task(ParallelSandbox sandbox, PendingBranch pendingBranch)
                : base(true)
            {
                m_sandbox = sandbox;
                m_currentBranch = pendingBranch;

                if (m_currentBranch.Root != null)
                {
                    CurrentBranchDepth = m_currentBranch.Depth;
                    CurrentBranchOrder = m_currentBranch.Order;
                    m_currentBranch.Root.OverrideController(this);
                    m_currentBranch.Root.RunTurnFromMainPhase();
                }
                else
                {
                    CurrentBranchDepth = 0;
                    CurrentBranchOrder = 0;
                    m_currentBranch.Root = m_sandbox.RootGame.CloneWithController(this);
                    m_currentBranch.Root.RunTurn();
                }

                m_sandbox.AddResult(m_currentBranch.Root, m_currentBranch.ChoicePath);
            }

            private PendingBranch ForkBranch(Choice choice)
            {
                var newChoicePath = new Choice[CurrentBranchChoicePath.Length + 1];
                Array.Copy(CurrentBranchChoicePath, newChoicePath, CurrentBranchChoicePath.Length);
                newChoicePath[CurrentBranchChoicePath.Length] = choice;
                return new PendingBranch { ChoicePath = newChoicePath };
            }

            private void OnInteraction(Interactions.BaseInteraction io, IEnumerable<Choice> choices)
            {
                ++CurrentBranchDepth;

                if (!ChoiceMade)
                {
                    bool isTacticalPhase = io is Interactions.TacticalPhase;
                    PendingBranch firstBranch = null;

                    foreach (var choice in choices)
                    {
                        var branch = ForkBranch(choice);

                        if (isTacticalPhase)
                        {
                            // create save point
                            branch.Root = io.Game.Clone();
                            branch.Depth = CurrentBranchDepth - 1;
                            branch.Order = CurrentBranchOrder;
                        }

                        if (firstBranch == null)
                        {
                            firstBranch = branch;
                        }
                        else
                        {
                            m_sandbox.StartBranch(branch);
                        }
                    }

                    if (firstBranch != null)
                    {
                        m_currentBranch = firstBranch;
                    }
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
                OnInteraction(interactionObj, m_sandbox.m_simulator.TacticalPhase(interactionObj, this));
                return false;
            }

            [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
            private bool OnSelectCards(Interactions.SelectCards interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.SelectCards(interactionObj, this));
                return false;
            }

            [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
            private bool OnMessageBox(Interactions.MessageBox interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.MessageBox(interactionObj, this));
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
}
