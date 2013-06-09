using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    class Passive_FirstStrike_Defender :
        BaseBehavior<Passive_FirstStrike_Defender.ModelType>,
        Commands.ICause,
        IPreemptiveTrigger<Commands.DealDamageToCard>
    {
        ResolveContext IPreemptiveTrigger<Commands.DealDamageToCard>.RunPreemptive(Commands.DealDamageToCard command, bool firstTimeTriggering)
        {
            if (firstTimeTriggering
                && command.Target == Host
                && Host.IsOnBattlefield
                && command.Cause is Warrior
                && Host.Behaviors.Has<Warrior>())
            {
                var ctx = Game.CreateResolveContext();
                ctx.QueueCommands(new Commands.DealDamageToCard((command.Cause as Warrior).Host, Host.Behaviors.Get<Warrior>().Attack, this));
                return ctx;
            }

            return null;
        }

        [BehaviorModel(typeof(Passive_FirstStrike_Defender), Category = "v0.5/Passive", DefaultName = "先攻（防御者）")]
        public class ModelType : BehaviorModel
        { }
    }
}
