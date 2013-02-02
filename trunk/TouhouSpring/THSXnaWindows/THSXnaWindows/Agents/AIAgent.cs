using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent : BaseAgent
    {
        private struct CardScorePair
        {
            public BaseCard Card;
            public double Score;
        }

        private struct ScoredBranch : IComparable<ScoredBranch>
        {
            public Simulation.Context.Branch Branch;
            public double Score;

            public int CompareTo(ScoredBranch other)
            {
                return Score.CompareTo(other.Score);
            }
        }

        private Game.BackupPoint m_lastBackupPoint;
        private Game m_lastBackupedGame;
        private Simulation.Context.Branch m_stage1Plan;
        private Simulation.Context.Branch m_stage2Plan;
        private Simulation.Context.Branch m_otherPlan;

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            // sacrifice
            var sacrifice = Sacrifice_MakeChoice2(io);
            if (sacrifice != null)
            {
                Debug.WriteLine("Sacrifice: " + sacrifice.Model.Name);
                io.RespondSacrifice(sacrifice);
                return;
            }

            if (m_stage1Plan == null)
            {
                var simulationCtx = new Simulation.Context(io.Game, new Simulation.MainStage1Simulator());
                simulationCtx.Start(game => game.RunMainPhase());

                int pid = io.Game.Players.IndexOf(io.Player);
                var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                m_stage1Plan = scoredBranches.Max().Branch;

                Debug.Assert(m_stage1Plan.ChoicePath.Last() is Simulation.PassChoice);
                m_stage1Plan.ChoicePath.RemoveAt(m_stage1Plan.ChoicePath.Count - 1);

                Debug.WriteLine("Stage1Plan:");
                PrintEvaluate(m_stage1Plan.Result.Players[pid]);
            }

            if (m_stage1Plan.ChoicePath.Count > 0)
            {
                MakeChoice(m_stage1Plan, io);
                return;
            }

            if (m_stage2Plan == null)
            {
                var simulationCtx = new Simulation.Context(io.Game, new Simulation.MainStage2Simulator());
                simulationCtx.Start(game => game.RunMainPhase());

                int pid = io.Game.Players.IndexOf(io.Player);
                var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                m_stage2Plan = scoredBranches.Max().Branch;

                Debug.Assert(m_stage2Plan.ChoicePath.Last() is Simulation.PassChoice);
                m_stage2Plan.ChoicePath.RemoveAt(m_stage2Plan.ChoicePath.Count - 1);

                Debug.WriteLine("Stage2Plan:");
                PrintEvaluate(m_stage2Plan.Result.Players[pid]);
            }

            if (m_stage2Plan.ChoicePath.Count > 0)
            {
                MakeChoice(m_stage2Plan, io);
                return;
            }

            m_stage1Plan = null;
            m_stage2Plan = null;

            Debug.WriteLine("Pass");
            io.RespondPass();
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            Simulation.Context.Branch plan;

            if (m_stage1Plan != null && m_stage1Plan.ChoicePath.Count > 0)
            {
                plan = m_stage1Plan;
            }
            else if (m_stage2Plan != null && m_stage2Plan.ChoicePath.Count > 0)
            {
                plan = m_stage2Plan;
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

                    Debug.WriteLine("OtherPlan:");
                    PrintEvaluate(m_otherPlan.Result.Players[pid]);
                }
                plan = m_otherPlan;
            }

            Debug.Assert(plan.ChoicePath[0] is Simulation.SelectCardChoice);
            MakeChoice(plan, io);
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

        private void MakeChoice(Simulation.Context.Branch plan, Interactions.BaseInteraction io)
        {
            Debug.Assert(plan.ChoicePath.Count > 0);
            Debug.WriteLine(plan.ChoicePath[0].Print(io));
            plan.ChoicePath[0].Make(io);
            plan.ChoicePath.RemoveAt(0);
        }
    }
}
