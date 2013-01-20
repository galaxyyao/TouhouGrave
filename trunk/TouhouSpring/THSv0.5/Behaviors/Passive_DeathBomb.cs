using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeathBomb :
        BaseBehavior<Passive_DeathBomb.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.Resolve>
    {
        private Warrior m_fatalWarriorCause;
        // 1, listen to DealDamageToCard command
        // if a fatal damage is dealt by Warrior, set m_fatalWarriorCause to this warrior;
        // 2, listen to Resolve command
        // if the card's life > 0, cancel the whole process
        // otherwise the bomb effect is triggered
        // 3, m_fatalWarriorCause is cleared to null

        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (command.Target == Host && command.Cause is Warrior)
            {
                var warrior = Host.Behaviors.Get<Warrior>();
                if (warrior != null
                    && warrior.Life > -command.DamageToDeal
                    && warrior.Life <= 0)
                {
                    m_fatalWarriorCause = command.Cause as Warrior;
                }
            }
        }

        public void RunEpilog(Commands.Resolve command)
        {
            if (m_fatalWarriorCause != null
                && Host.Behaviors.Get<Warrior>().Life <= 0)
            {
                Game.IssueCommands(new Commands.DealDamageToCard(m_fatalWarriorCause.Host, Model.Damage, this));
            }
            m_fatalWarriorCause = null;
        }

        [BehaviorModel(typeof(Passive_DeathBomb), Category = "v0.5/Passive", DefaultName = "死后炸弹")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
