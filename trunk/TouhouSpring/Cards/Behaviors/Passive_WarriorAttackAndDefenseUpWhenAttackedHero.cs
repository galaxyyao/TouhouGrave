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
                context.Game.IssueCommands(new AddBehavior
                {
                    Target = Host,
                    Behavior = new Enhance(1, 1)
                });
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackAndDefenseUpWhenAttackedHero), DefaultName = "午后红茶")]
        public class ModelType : BehaviorModel
        { }
    }
}
