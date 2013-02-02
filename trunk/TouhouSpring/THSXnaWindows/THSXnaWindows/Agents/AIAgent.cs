using System;
using System.Collections.Generic;
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

        private Simulation.Context.Branch m_stage1Plan;
        private Simulation.Context.Branch m_stage2Plan;

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            // sacrifice
            var sacrifice = Sacrifice_MakeChoice2(io);
            if (sacrifice != null)
            {
                System.Diagnostics.Debug.WriteLine("Sacrifice: " + sacrifice.Model.Name);
                io.RespondSacrifice(sacrifice);
                return;
            }

            if (m_stage1Plan == null)
            {
                var simulationCtx = new Simulation.Context(io.Game, new Simulation.MainStage1Simulator());
                simulationCtx.Start();

                int pid = io.Game.Players.IndexOf(io.Player);
                var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                m_stage1Plan = scoredBranches.Max().Branch;

                System.Diagnostics.Debug.Assert(m_stage1Plan.ChoicePath.Last() is Simulation.PassChoice);
                m_stage1Plan.ChoicePath.RemoveAt(m_stage1Plan.ChoicePath.Count - 1);
            }

            if (m_stage1Plan.ChoicePath.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine(m_stage1Plan.ChoicePath[0].Print(io));
                m_stage1Plan.ChoicePath[0].Make(io);
                m_stage1Plan.ChoicePath.RemoveAt(0);
                return;
            }

            m_stage1Plan = null;

            if (m_stage2Plan == null)
            {
                var simulationCtx = new Simulation.Context(io.Game, new Simulation.MainStage2Simulator());
                simulationCtx.Start();

                int pid = io.Game.Players.IndexOf(io.Player);
                var scoredBranches = simulationCtx.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
                m_stage2Plan = scoredBranches.Max().Branch;

                System.Diagnostics.Debug.Assert(m_stage2Plan.ChoicePath.Last() is Simulation.PassChoice);
                m_stage2Plan.ChoicePath.RemoveAt(m_stage2Plan.ChoicePath.Count - 1);
            }

            if (m_stage2Plan.ChoicePath.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine(m_stage2Plan.ChoicePath[0].Print(io));
                m_stage2Plan.ChoicePath[0].Make(io);
                m_stage2Plan.ChoicePath.RemoveAt(0);
                return;
            }

            m_stage2Plan = null;

            System.Diagnostics.Debug.WriteLine("Pass");
            io.RespondPass();
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            io.Respond(io.SelectFromSet.Count != 0
                       ? new BaseCard[1] { io.SelectFromSet[0] }.ToIndexable()
                       : Indexable.Empty<BaseCard>());
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
