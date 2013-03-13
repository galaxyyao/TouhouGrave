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
            public Simulation.Branch Branch;
            public double Score;

            public int CompareTo(ScoredBranch other)
            {
                return Score.CompareTo(other.Score);
            }
        }

        private Messaging.LetterBox m_letterbox = new Messaging.LetterBox();
        private Simulation.Branch m_plan = null;
        private int m_planProgress = 0;

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

        public override void OnSelectNumber(Interactions.SelectNumber io)
        {
            new Messaging.Message("OnSelectNumber", io).SendTo(m_letterbox);
        }

        public override bool OnTurnStarted(Interactions.NotifyPlayerEvent io)
        {
            new Messaging.Message("BeginSimulation", io.Game.Clone()).SendTo(m_letterbox);
            return false;
        }

        private void AIThread()
        {
            while (true)
            {
                var msg = m_letterbox.WaitForNextMessage();

                switch (msg.Text)
                {
                    case "BeginSimulation":
                        MakePlan(msg.Attachment as Game);
                        break;

                    case "OnTacticalPhase":
                    case "OnSelectCards":
                    case "OnSelectNumber":
                        Debug.Assert(m_plan != null);
                        RespondInteraction(msg.Attachment as Interactions.BaseInteraction);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private void MakePlan(Game game)
        {
            Debug.Assert(m_plan == null);
            long startTime, endTime, freq;

            PInvokes.Kernel.QueryPerformanceFrequency(out freq);
            PInvokes.Kernel.QueryPerformanceCounter(out startTime);

            Simulation.ISandbox simSandbox = new Simulation.StpSandbox(game, new Simulation.MainPhaseSimulator());
            simSandbox.Run();

            var pid = (GameApp.Service<Services.GameManager>().Game.Controller as XnaUIController).Agents.IndexOf(this);
            var scoredBranches = simSandbox.Branches.Select(branch => new ScoredBranch { Branch = branch, Score = Evaluate(branch.Result, pid) });
            m_plan = scoredBranches.Max().Branch;
            m_planProgress = 0;

            PInvokes.Kernel.QueryPerformanceCounter(out endTime);

            Trace.WriteLine(String.Format("Plan (total {0}, {1:0.000}ms)", simSandbox.BranchCount, (double)(endTime - startTime) / (double)freq * 1000.0));
            PrintEvaluate(m_plan.Result.Players[pid]);
        }

        private void RespondInteraction(Interactions.BaseInteraction io)
        {
            Debug.Assert(m_plan != null && m_plan.ChoicePath.Length > m_planProgress);
            Trace.WriteLine("\t" + m_plan.ChoicePath[m_planProgress].Print(io));
            m_plan.ChoicePath[m_planProgress++].Make(io);

            if (m_plan.ChoicePath.Length == m_planProgress)
            {
                m_plan = null;
            }
        }
    }
}
