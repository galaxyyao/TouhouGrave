using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class MassiveAttack : BaseBehavior<MassiveAttack.ModelType>,
        IGlobalPrologTrigger<Commands.DealDamageToCard>,
        Commands.ICause
    {
        void IGlobalPrologTrigger<Commands.DealDamageToCard>.RunGlobalProlog(Commands.DealDamageToCard command)
        {
            // queue commands in Prolog so they are in the same "timing group"
            if (command.Cause is Warrior
                && command.Cause == Host.Warrior)
            {
                foreach (var target in Game.Players.Where(p => p != Host.Owner).SelectMany(p => p.CardsOnBattlefield))
                {
                    if (target.Warrior != null && target != command.Target)
                    {
                        Game.QueueCommands(new Commands.DealDamageToCard(target, Host.Warrior.Attack, this));
                    }
                }
            }
        }

        [BehaviorModel(typeof(MassiveAttack), Category = "v0.5/Passive", DefaultName = "核热")]
        public class ModelType : BehaviorModel
        { }
    }
}
