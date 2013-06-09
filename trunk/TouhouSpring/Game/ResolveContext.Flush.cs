using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class ResolveContext
    {
        // a dictionary of cached command runners to avoid using reflection too intensively
        private static Dictionary<Type, ICommandRunner> s_commandRunnerCache = new Dictionary<Type, ICommandRunner>();

        internal void FlushCommandQueue()
        {
            Debug.Assert(RunningCommand == null);

            if (m_commandQueueHead is Commands.IInitiativeCommand)
            {
                // collect conditions: resources + targets
                var result = RunPrerequisite(m_commandQueueHead, true);
                if (result.Canceled)
                {
                    // all remaining commands are discarded
                    Game.Controller.OnCommandCanceled(RunningCommand, result.Reason);
                    return;
                }
            }

            Debug.Assert(RunningCommand == null);

            // run resource commands (main+epilog)
            for (Commands.BaseCommand cmd = null; (cmd = DequeResourceCommand()) != null; )
            {
                RunMainAndEpilog(cmd);
            }

            // run other commands (prolog+premptive+main+epilog)
            bool commandFlushed = false;
            while (m_commandQueueHead != null)
            {
                commandFlushed = true;
                while (m_commandQueueHead != null)
                {
                    RunFull(DequeCommand());
                    if (Abort)
                    {
                        return;
                    }
                }

                QueueCommand(new Commands.Resolve());
                var resolveCmd = DequeCommand();
                Debug.Assert(resolveCmd is Commands.Resolve);
                RunMainAndEpilog(resolveCmd);
            }

            if (commandFlushed)
            {
                Game.Controller.OnCommandFlushed();
            }
        }
    }
}
