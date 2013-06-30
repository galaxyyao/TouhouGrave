using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    class Passive_FirstStrike_Defender :
        BaseBehavior<Passive_FirstStrike_Defender.ModelType>,
        Commands.ICause,
        ILocalPreemptiveTrigger<Commands.DealDamageToCard>
    {
        ResolveContext ILocalPreemptiveTrigger<Commands.DealDamageToCard>.RunLocalPreemptive(Commands.DealDamageToCard command, bool firstTimeTriggering)
        {
            if (firstTimeTriggering
                && Host.IsOnBattlefield
                && command.Cause is Warrior
                && Host.Warrior != null)
            {
                var ctx = Game.CreateResolveContext();
                ctx.QueueCommands(new Commands.DealDamageToCard((command.Cause as Warrior).Host, Host.Warrior.Attack, this));
                return ctx;
            }

            return null;
        }

        [BehaviorModel(typeof(Passive_FirstStrike_Defender), Category = "v0.5/Passive", DefaultName = "先攻（防御者）")]
        public class ModelType : BehaviorModel
        { }
    }
}
