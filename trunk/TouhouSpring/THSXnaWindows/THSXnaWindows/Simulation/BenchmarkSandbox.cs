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
            public Interactions.TacticalPhase.CompiledResponse Response;
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
            : base(false, true)
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
                bool aborted = false;

                if (m_currentBranch.Root != null)
                {
                    CurrentBranchDepth = m_currentBranch.Depth;
                    CurrentBranchOrder = m_currentBranch.Order;
                    aborted = !m_currentBranch.Root.RunTurnFromMainPhase(m_currentBranch.Response);
                }
                else
                {
                    CurrentBranchDepth = 0;
                    CurrentBranchOrder = 0;
                    m_currentBranch.Root = RootGame.CloneWithController(this);
                    aborted = !m_currentBranch.Root.RunTurn();
                }

                if (!aborted)
                {
                    m_branches.Add(new Branch
                    {
                        ChoicePath = m_currentBranch.ChoicePath,
                        Result = m_currentBranch.Root
                    });
                }
            }
        }

        private PendingBranch ForkBranch(Choice choice)
        {
            var newChoicePath = new Choice[CurrentBranchChoicePath.Length + 1];
            Array.Copy(CurrentBranchChoicePath, newChoicePath, CurrentBranchChoicePath.Length);
            newChoicePath[CurrentBranchChoicePath.Length] = choice;
            return new PendingBranch { ChoicePath = newChoicePath };
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
                var mainPhase = io as Interactions.TacticalPhase;
                PendingBranch firstBranch = null;

                foreach (var choice in choices)
                {
                    if (mainPhase != null && choice is KillBranchChoice)
                    {
                        continue;
                    }

                    var branch = ForkBranch(choice);

                    if (mainPhase != null)
                    {
                        // create save point
                        branch.Root = io.Game.CloneWithController(this);
                        branch.Depth = CurrentBranchDepth;
                        branch.Order = Math.Max(choice.Order, CurrentBranchOrder);

                        if (choice is PlayCardChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondPlay(
                                mainPhase.PlayCardCandidates[(choice as PlayCardChoice).CardIndex]);
                        }
                        else if (choice is ActivateAssistChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondActivate(
                                mainPhase.ActivateAssistCandidates[(choice as ActivateAssistChoice).CardIndex]);
                        }
                        else if (choice is CastSpellChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondCast(
                                mainPhase.CastSpellCandidates[(choice as CastSpellChoice).SpellIndex]);
                        }
                        else if (choice is SacrificeChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondSacrifice(
                                mainPhase.SacrificeCandidates[(choice as SacrificeChoice).CardIndex]);
                        }
                        else if (choice is RedeemChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondRedeem(
                                mainPhase.RedeemCandidates[(choice as RedeemChoice).CardIndex]);
                        }
                        else if (choice is AttackCardChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondAttackCard(
                                mainPhase.AttackerCandidates[(choice as AttackCardChoice).AttackerIndex],
                                mainPhase.DefenderCandidates[(choice as AttackCardChoice).DefenderIndex]);
                        }
                        else if (choice is AttackPlayerChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondAttackPlayer(
                                mainPhase.AttackerCandidates[(choice as AttackPlayerChoice).AttackerIndex],
                                mainPhase.Game.Players[(choice as AttackPlayerChoice).PlayerIndex]);
                        }
                        else if (choice is PassChoice)
                        {
                            branch.Response = mainPhase.CompiledRespondPass();
                        }
                    }

                    if (firstBranch == null)
                    {
                        firstBranch = branch;
                    }
                    else
                    {
                        m_pendingBranches.Add(branch);
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
                // no choice generated
                System.Diagnostics.Debug.Assert(io is Interactions.TacticalPhase);
                (io as Interactions.TacticalPhase).RespondAbort();
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

        [Interactions.MessageHandler(typeof(Interactions.SelectNumber))]
        private bool OnSelectNumber(Interactions.SelectNumber interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.SelectNumber(interactionObj, this));
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
