using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_BloodThirsty:
        BaseBehavior<Passive_BloodThirsty.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.DealDamageToPlayer>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.DamageToDeal > 0
                && Host.IsOnBattlefield
                && command.Cause==Host.Behaviors.Get<Warrior>())
            {
                Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 1);
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        void IEpilogTrigger<Commands.DealDamageToPlayer>.Run(Commands.DealDamageToPlayer command)
        {
            if (command.DamageToDeal > 0
                && Host.IsOnBattlefield
                && command.Cause == Host.Behaviors.Get<Warrior>())
            {
                Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 1);
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        [BehaviorModel(typeof(Passive_BloodThirsty), DefaultName = "嗜血")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
