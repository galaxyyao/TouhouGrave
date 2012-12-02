using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackAndDefenseUpWhenAttackedHero:
        BaseBehavior<Passive_WarriorAttackAndDefenseUpWhenAttackedHero.ModelType>,
        IEpilogTrigger<DealDamageToPlayer>
    {
        void IEpilogTrigger<DealDamageToPlayer>.Run(CommandContext<DealDamageToPlayer> context)
        {
            if (context.Command.Cause == Host)
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //var attackMod = new AttackModifier(x => x + 1);
                //var defenseMod = new DefenseModifier(y => y + 1);
                //Host.Behaviors.Add(attackMod);
                //Host.Behaviors.Add(defenseMod);
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackAndDefenseUpWhenAttackedHero), DefaultName = "午后红茶")]
        public class ModelType : BehaviorModel
        { }
    }
}
