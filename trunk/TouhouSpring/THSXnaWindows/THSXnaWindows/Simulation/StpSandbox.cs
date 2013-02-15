using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amib.Threading;

namespace TouhouSpring.Simulation
{
    class StpSandbox : ParallelSandbox
    {
        private static SmartThreadPool s_stp;

        public StpSandbox(Game game, BaseSimulator simulator)
            : base(game, simulator)
        { }

        public override void Run()
        {
            s_stp.QueueWorkItem(() => new Task(this, new PendingBranch { ChoicePath = new Choice[0] }));
            s_stp.WaitForIdle();
        }

        protected override void StartBranch(PendingBranch branch)
        {
            s_stp.QueueWorkItem(() => new Task(this, branch));
        }

        static StpSandbox()
        {
            if (Environment.ProcessorCount > 1)
            {
                s_stp = new SmartThreadPool(new STPStartInfo
                {
                    MinWorkerThreads = Math.Min(Environment.ProcessorCount - 1, 4),
                    MaxWorkerThreads = Math.Min(Environment.ProcessorCount - 1, 4),
                    ThreadAffinity = (IntPtr)0xfe // spare the first core
                });
            }
            else
            {
                s_stp = new SmartThreadPool(new STPStartInfo
                {
                    MinWorkerThreads = 1,
                    MaxWorkerThreads = 1
                });
            }
        }
    }
}
