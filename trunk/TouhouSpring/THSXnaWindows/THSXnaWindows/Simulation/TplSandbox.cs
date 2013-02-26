using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tpl = System.Threading.Tasks;

namespace TouhouSpring.Simulation
{
    class TplSandbox : ParallelSandbox
    {
        public TplSandbox(Game game, BaseSimulator simulator)
            : base(game, simulator)
        { }

        public override void Run()
        {
            var task = Tpl.Task.Factory.StartNew(() => new Task(this, new PendingBranch { ChoicePath = new Choice[0] }));
            task.Wait();
        }

        protected override void StartBranch(PendingBranch branch)
        {
            Tpl.Task.Factory.StartNew(() => new Task(this, branch), Tpl.TaskCreationOptions.AttachedToParent);
        }
    }
}
