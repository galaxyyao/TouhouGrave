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
                Game.IssueCommands(new Commands.AddBehavior(Host, new Enhance(1)));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackAndDefenseUpWhenAttackedHero), DefaultName = "午后红茶")]
        public class ModelType : BehaviorModel
        { }
    }
}
