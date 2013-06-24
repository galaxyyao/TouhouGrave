using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    class Passive_FirstStrike_Attacker :
        BaseBehavior<Passive_FirstStrike_Attacker.ModelType>,
        Commands.ICause,
        IGlobalPreemptiveTrigger<Commands.DealDamageToCard>
    {
        ResolveContext IGlobalPreemptiveTrigger<Commands.DealDamageToCard>.RunGlobalPreemptive(Commands.DealDamageToCard command, bool firstTimeTriggering)
        {
            if (firstTimeTriggering
                && Host.IsOnBattlefield
                && command.Cause is Warrior
                && (command.Cause as Warrior).Host == Host)
            {
                if (!Model.DoubleStrike)
                {
                    command.PatchDamageToDeal(0);
                }

                var ctx = Game.CreateResolveContext();
                ctx.QueueCommands(new Commands.DealDamageToCard(command.Target, Host.Behaviors.Get<Warrior>().Attack, this));
                return ctx;
            }

            return null;
        }

        [BehaviorModel(typeof(Passive_FirstStrike_Attacker), Category = "v0.5/Passive", DefaultName = "先攻（攻击者）")]
        public class ModelType : BehaviorModel
        {
            public bool DoubleStrike { get; set; }
        }
    }
}
