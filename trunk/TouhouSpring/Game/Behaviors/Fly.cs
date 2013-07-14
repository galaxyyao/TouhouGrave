using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Fly : BaseBehavior<Fly.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.CastSpell>,
        IGlobalEpilogTrigger<Commands.EndPhase>,
        ICastableSpell
    {
        bool m_isFlyStatusChanged = false;

        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            m_isFlyStatusChanged = false;
        }

        public CommandResult RunLocalPrerequisite(Commands.CastSpell command)
        {
            if (m_isFlyStatusChanged)
                return CommandResult.Cancel("Already changed flying status");
            return CommandResult.Pass;
        }

        public void RunSpell(Commands.CastSpell command)
        {
            if (Host.Behaviors.Has<Flying>())
            {
                Game.QueueCommands(new Commands.RemoveBehavior(Host, Host.Behaviors.Get<Flying>()));
            }
            else
            {
                var flying = new Flying.ModelType().CreateInitialized();
                Game.QueueCommands(new Commands.AddBehavior(Host, flying));
            }
            m_isFlyStatusChanged = true;
        }

        [BehaviorModel(typeof(Fly), Category = "Core", DefaultName = "切换飞行/降落")]
        public class ModelType : BehaviorModel
        { }
    }
}
