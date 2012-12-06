using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackAndDefenseUpWhenAttackedHero:
        BaseBehavior<Passive_WarriorAttackAndDefenseUpWhenAttackedHero.ModelType>,
        IEpilogTrigger<Commands.DealDamageToPlayer>
    {
        void IEpilogTrigger<Commands.DealDamageToPlayer>.Run(CommandContext<Commands.DealDamageToPlayer> context)
        {
            if (context.Command.Cause == Host)
            {
                context.Game.IssueCommands(new Commands.AddBehavior
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
