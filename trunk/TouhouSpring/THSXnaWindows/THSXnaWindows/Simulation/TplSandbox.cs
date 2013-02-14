using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    partial class TplSandbox : ISandbox
    {
        private BaseSimulator m_simulator;
        private ConcurrentBag<Branch> m_branches = new ConcurrentBag<Branch>();

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

        public TplSandbox(Game game, BaseSimulator simulator)
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
            var task = SimulationTask.Start(this);
            task.Wait();
        }

        private void AddResult(Game finalState, Choice[] choicePath)
        {
            m_branches.Add(new Branch { Result = finalState, ChoicePath = choicePath });
        }
    }
}
