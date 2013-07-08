using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class PaidDrawOnKill : BaseBehavior<PaidDrawOnKill.ModelType>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        Commands.ICause
    {
        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.Subject != null
                && command.Subject.Owner == Host.Owner
                && command.Subject.Warrior != null
                && command.ToZone == SystemZone.Graveyard)
            {
                var realManaCost = Host.Owner.CalculateFinalManaSubtract(Model.ManaCost);
                if (Host.Owner.Mana >= realManaCost)
                {
                    Game.QueueCommands(
                        new Commands.SubtractPlayerMana(Host.Owner, realManaCost, true, this),
                        new Commands.DrawMove(Host.Owner, SystemZone.Hand));
                }
            }
        }

        [BehaviorModel(typeof(PaidDrawOnKill), Category = "v0.5/Assist")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }

            public ModelType()
            {
                ManaCost = 1;
            }
        }
    }
}
