using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class Game
    {
        private Stack<ResolveContext> m_resolveContextStack = new Stack<ResolveContext>(1);

        public ResolveContext CreateResolveContext()
        {
            return new ResolveContext(this);
        }

        internal void StackAndFlush(ResolveContext ctx)
        {
            Debug.Assert(ctx.Game == this);
            Debug.Assert(!m_resolveContextStack.Contains(ctx));
            Debug.Assert(m_resolveContextStack.Count == 0
                         || (m_resolveContextStack.Peek().RunningCommand != null
                             && m_resolveContextStack.Peek().RunningCommand.ExecutionPhase == Commands.CommandPhase.Preemptive));
            m_resolveContextStack.Push(ctx);
            ctx.FlushCommandQueue();
            m_resolveContextStack.Pop();
        }

        internal void StackAndFlush(params Commands.BaseCommand[] commands)
        {
            var ctx = new ResolveContext(this);
            ctx.QueueCommands(commands);
            StackAndFlush(ctx);
        }
    }
}
