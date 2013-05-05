using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class Game
    {
        private Stack<ResolveContext> m_resolveContextStack = new Stack<ResolveContext>();

        internal ResolveContext PushNewResolveContext(params Commands.BaseCommand[] commands)
        {
            // TODO: check the timing
            var ctx = new ResolveContext(this);
            ctx.QueueCommands(commands);
            m_resolveContextStack.Push(ctx);
            return ctx;
        }

        internal void FlushResolveContext()
        {
            m_resolveContextStack.Peek().FlushCommandQueue();
            m_resolveContextStack.Pop();
        }
    }
}
