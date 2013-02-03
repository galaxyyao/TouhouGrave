using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent : BaseAgent
    {
        private struct ScoredBranch : IComparable<ScoredBranch>
        {
            public Simulation.Context.Branch Branch;
            public double Score;

            public int CompareTo(ScoredBranch other)
            {
                return Score.CompareTo(other.Score);
            }
        }

        private Messaging.LetterBox m_letterbox = new Messaging.LetterBox();

        private Game.BackupPoint m_lastBackupPoint;
        private Game m_lastBackupedGame;
        private Simulation.Context.Branch m_mainPlan;
        private Simulation.Context.Branch m_otherPlan;

        public AIAgent()
        {
            var thread = new System.Threading.Thread(AIThread)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            new Messaging.Message("OnTacticalPhase", io).SendTo(m_letterbox);
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            if (io.Mode != Interactions.SelectCards.SelectMode.Single)
            {
                throw new NotImplementedException("Multiple selection is not implemented yet.");
            }
            new Messaging.Message("OnSelectCards", io).SendTo(m_letterbox);
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            if (io.Buttons == Interactions.MessageBoxButtons.OK)
            {
                io.Respond(Interactions.MessageBoxButtons.OK);
                return;
            }

            throw new NotImplementedException();
        }

        public override void OnGameBackup(Game.BackupPoint backupPoint, Game game)
        {
            m_lastBackupPoint = backupPoint;
            m_lastBackupedGame = game.Clone();
        }

        private void AIThread()
        {
            while (true)
            {
                var msg = m_letterbox.WaitForNextMessage();
                var io = msg.Attachment as Interactions.BaseInteraction;

                switch (msg.Text)
                {
                    case "OnTacticalPhase":
                        TacticalPhasePlanner(io as Interactions.TacticalPhase);
                        break;

                    case "OnSelectCards":
                        SelectCardsPlanner(io as Interactions.SelectCards);
                        break;

                    default:
                        break;
                }
            }
        }

        private void TacticalPhasePlanner(Interactions.TacticalPhase io)
        {
            if (m_mainPlan == null)
            {
                var simulationCtx = new Simulation.Context(io.Game, new Simulation.MainPhaseSimulator());
                simulationCtx.Start(game => game.RunMainPhase());

                int pid = io.Game.Players.IndexOf(io.Player);
                var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                m_mainPlan = scoredBranches.Max().Branch;

                Debug.Assert(m_mainPlan.ChoicePath.Last() is Simulation.PassChoice);
                m_mainPlan.ChoicePath.RemoveAt(m_mainPlan.ChoicePath.Count - 1);

                Debug.WriteLine(String.Format("MainPlan (total {0}):", simulationCtx.BranchCount));
                PrintEvaluate(m_mainPlan.Result.Players[pid]);
            }

            if (m_mainPlan.ChoicePath.Count > 0)
            {
                MakeChoice(m_mainPlan, io);
                return;
            }

            m_mainPlan = null;

            Debug.WriteLine("Pass");
            io.RespondPass();
        }

        private void SelectCardsPlanner(Interactions.SelectCards io)
        {
            Simulation.Context.Branch plan;

            if (m_mainPlan != null && m_mainPlan.ChoicePath.Count > 0)
            {
                plan = m_mainPlan;
            }
            else
            {
                if (m_otherPlan == null || m_otherPlan.ChoicePath.Count == 0)
                {
                    var simulationCtx = new Simulation.Context(m_lastBackupedGame, new Simulation.NonMainSimulator());
                    if (m_lastBackupPoint == Game.BackupPoint.PreMain)
                    {
                        simulationCtx.Start(game => game.RunPreMainPhase());
                    }
                    else
                    {
                        simulationCtx.Start(game => game.RunPostMainPhase());
                    }

                    int pid = io.Game.Players.IndexOf(io.Player);
                    var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                    m_otherPlan = scoredBranches.Max().Branch;

                    Debug.WriteLine(String.Format("OtherPlan (total {0}):", simulationCtx.BranchCount));
                    PrintEvaluate(m_otherPlan.Result.Players[pid]);
                }
                plan = m_otherPlan;
            }

            Debug.Assert(plan.ChoicePath[0] is Simulation.SelectCardChoice);
            MakeChoice(plan, io);
        }

        private void MakeChoice(Simulation.Context.Branch plan, Interactions.BaseInteraction io)
        {
            Debug.Assert(plan.ChoicePath.Count > 0);
            Debug.WriteLine(plan.ChoicePath[0].Print(io));
            plan.ChoicePath[0].Make(io);
            plan.ChoicePath.RemoveAt(0);
        }
    }
}
