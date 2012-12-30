using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorDefenseDownWhenAttacked
        : BaseBehavior<Passive_WarriorDefenseDownWhenAttacked.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Target == Host
                && command.Cause is Warrior
                && command.DamageToDeal > 0)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Host.Behaviors.Get<Warrior>(),
                    "DefenseModifiers",
                    new object[] { "add", new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, -1) }));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDefenseDownWhenAttacked), DefaultName = "日光折射")]
        public class ModelType : BehaviorModel
        { }
    }
}
