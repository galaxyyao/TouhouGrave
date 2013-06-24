using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_BloodThirsty:
        BaseBehavior<Passive_BloodThirsty.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.SubtractPlayerLife>
    {
        public void RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.DamageToDeal > 0
                && Host.IsOnBattlefield
                && command.Cause == Host.Behaviors.Get<Warrior>())
            {
                var m_attackMod = new ValueModifier(ValueModifierOperator.Add, 1);
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        public void RunGlobalEpilog(Commands.SubtractPlayerLife command)
        {
            if (command.FinalAmount > 0
                && Host.IsOnBattlefield
                && command.Cause == Host.Behaviors.Get<Warrior>())
            {
                var m_attackMod = new ValueModifier(ValueModifierOperator.Add, 1);
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        [BehaviorModel(typeof(Passive_BloodThirsty), Category = "v0.5/Passive", DefaultName = "嗜血")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
