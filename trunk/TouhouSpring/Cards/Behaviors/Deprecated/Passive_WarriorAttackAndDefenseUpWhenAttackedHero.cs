using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorAttackAndDefenseUpWhenAttackedHero :
        BaseBehavior<Passive_WarriorAttackAndDefenseUpWhenAttackedHero.ModelType>,
        IEpilogTrigger<Commands.DealDamageToPlayer>
    {
        public void RunEpilog(Commands.DealDamageToPlayer command)
        {
            if (command.Cause == Host)
            {
                Game.QueueCommands(new Commands.AddBehavior(Host, new Enhance.ModelType { AttackBoost = 1 }.CreateInitialized()));
            }
        }

        [BehaviorModel(DefaultName = "午后红茶")]
        public class ModelType : BehaviorModel<Passive_WarriorAttackAndDefenseUpWhenAttackedHero>
        { }
    }
}
