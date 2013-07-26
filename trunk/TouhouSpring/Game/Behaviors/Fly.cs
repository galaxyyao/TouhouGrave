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
        ILocalEpilogTrigger<Commands.PlayCard>,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.SubtractPlayerLife>,
        ICastableSpell
    {
        bool m_isFlyStatusChanged = false;
        private IBehavior m_flying;
        private IBehavior m_unattackable;

        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (Game.ActingPlayer == Host.Owner && command.PreviousPhase == "Main")
            {
                m_isFlyStatusChanged = false;
            }
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.AddBehavior(Host, m_flying));
            Game.QueueCommands(new Commands.RemoveBehavior(Host, m_unattackable));
        }

        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (Host.Behaviors.Contains(m_flying)
                && command.Cause == Host.Warrior)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_flying));
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_unattackable));
                m_isFlyStatusChanged = true;
            }
        }

        void IGlobalEpilogTrigger<Commands.SubtractPlayerLife>.RunGlobalEpilog(Commands.SubtractPlayerLife command)
        {
            if (Host.Behaviors.Contains(m_flying)
                && command.Cause == Host.Warrior)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_flying));
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_unattackable));
                m_isFlyStatusChanged = true;
            }
        }

        public CommandResult RunLocalPrerequisite(Commands.CastSpell command)
        {
            if (m_isFlyStatusChanged)
                return CommandResult.Cancel("Already changed flying status");
            return CommandResult.Pass;
        }

        protected override void OnInitialize()
        {
            m_flying = new Flying.ModelType().CreateInitialized();
            m_unattackable = new Unattackable.ModelType().CreateInitialized();
        }

        public void RunSpell(Commands.CastSpell command)
        {
            if (Host.Behaviors.Contains(m_flying))
            {
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_flying));
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_unattackable));
            }
            else
            {
                Game.QueueCommands(new Commands.AddBehavior(Host, m_flying));
                Game.QueueCommands(new Commands.AddBehavior(Host, m_unattackable));
            }
            m_isFlyStatusChanged = true;
        }

        [BehaviorModel(typeof(Fly), Category = "Core", DefaultName = "切换飞行/降落")]
        public class ModelType : BehaviorModel
        { }
    }
}
