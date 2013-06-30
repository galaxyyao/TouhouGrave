using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_OnDeath<T> : BaseBehavior<T>,
        ILocalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.Resolve>
        where T : IBehaviorModel
    {
        // 1, listen to DealDamageToCard command
        // if a fatal damage is dealt, set m_fatalDamageCause to it;
        // 2, listen to Resolve command
        // if the card's life > 0, cancel the whole process
        // otherwise OnDeath is triggered
        // 3, m_fatalDamageCause is cleared to null
        private IBehavior m_fatalDamageCause;

        void ILocalEpilogTrigger<Commands.DealDamageToCard>.RunLocalEpilog(Commands.DealDamageToCard command)
        {
            if (Host.Warrior != null
                && Host.Warrior.Life > -command.DamageToDeal
                && Host.Warrior.Life <= 0)
            {
                m_fatalDamageCause = command.Cause as IBehavior;
            }
        }

        void IGlobalEpilogTrigger<Commands.Resolve>.RunGlobalEpilog(Commands.Resolve command)
        {
            if (m_fatalDamageCause != null)
            {
                if (Host.Warrior.Life <= 0)
                {
                    OnDeath(m_fatalDamageCause, Host.Warrior);
                }
            }
            m_fatalDamageCause = null;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            if (m_fatalDamageCause != null)
            {
                throw new InvalidOperationException();
            }
        }

        protected virtual void OnDeath(IBehavior fatalDamageCause, Warrior hostWarrior) { }
    }
}
