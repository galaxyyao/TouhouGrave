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

            // save point at MainPhase
            public Game Root;
            public int Order;
            public int Depth;
            public Interactions.TacticalPhase.CompiledResponse Response;
        }

        protected class Task : BaseController, IContext
        {
            private const int BatchSize = 20;

            private ParallelSandbox m_sandbox;
            private PendingBranch m_currentBranch;

            private int m_pendingCursor = 0;
            private List<PendingBranch> m_pendingBranches = new List<PendingBranch>(BatchSize);

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
                : base(false, true)
            {
                m_sandbox = sandbox;
                m_pendingBranches.Add(pendingBranch);

                while (m_pendingCursor < m_pendingBranches.Count)
                {
                    m_currentBranch = m_pendingBranches[m_pendingCursor++];
                    bool aborted = false;

                    if (m_currentBranch.Root != null)
                    {
                        CurrentBranchDepth = m_currentBranch.Depth;
                        CurrentBranchOrder = m_currentBranch.Order;
                        m_currentBranch.Root.OverrideController(this);
                        aborted = !m_currentBranch.Root.RunTurnFromMainPhase(m_currentBranch.Response);
                    }
                    else
                    {
                        CurrentBranchDepth = 0;
                        CurrentBranchOrder = 0;
                        m_currentBranch.Root = m_sandbox.RootGame.CloneWithController(this);
                        aborted = !m_currentBranch.Root.RunTurn();
                    }

                    if (!aborted)
                    {
                        m_sandbox.AddResult(m_currentBranch.Root, m_currentBranch.ChoicePath);
                    }
                }
            }

            private PendingBranch ForkBranch(Choice choice)
            {
                var newChoicePath = new Choice[CurrentBranchChoicePath.Length + 1];
                for (int i = 0; i < CurrentBranchChoicePath.Length; ++i)
                {
                    newChoicePath[i] = CurrentBranchChoicePath[i];
                }
                newChoicePath[CurrentBranchChoicePath.Length] = choice;
                return new PendingBranch { ChoicePath = newChoicePath };
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
                            branch.Root = io.Game.Clone();
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
                            if (m_pendingBranches.Count < BatchSize)
                            {
                                m_pendingBranches.Add(branch);
                            }
                            else
                            {
                                m_sandbox.StartBranch(branch);
                            }
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

            protected override bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.TacticalPhase(interactionObj, this));
                return false;
            }

            protected override bool OnSelectCards(Interactions.SelectCards interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.SelectCards(interactionObj, this));
                return false;
            }

            protected override bool OnMessageBox(Interactions.MessageBox interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.MessageBox(interactionObj, this));
                return false;
            }

            protected override bool OnSelectNumber(Interactions.SelectNumber interactionObj)
            {
                OnInteraction(interactionObj, m_sandbox.m_simulator.SelectNumber(interactionObj, this));
                return false;
            }

            #endregion

            #region notifications

            protected override bool OnNotified(Interactions.NotifyCardEvent interactionObj)
            {
                interactionObj.Respond();
                return false;
            }

            protected override bool OnNotified(Interactions.NotifyGameEvent interactionObj)
            {
                interactionObj.Respond();
                return false;
            }

            protected override bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
            {
                interactionObj.Respond();
                return false;
            }

            protected override bool OnNotified(Interactions.NotifySpellEvent interactionObj)
            {
                interactionObj.Respond();
                return false;
            }

            #endregion
        }
    }
}
