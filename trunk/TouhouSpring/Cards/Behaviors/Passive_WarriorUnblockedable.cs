using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorUnblockedable:
        BaseBehavior<Passive_WarriorUnblockedable.ModelType>,
        ITrigger<Triggers.BlockPhaseStartedContext>
    {
        public void Trigger(Triggers.BlockPhaseStartedContext context)
        {
            if (context.Game.PlayerPlayer == Host.Owner)
            {
                foreach (var card in context.DeclaredAttackers)
                {
                    if (card != Host)
                        continue;
                    //TODO: Still not effective.
                    List<BaseCard> updatedDeclaredAttackers = context.DeclaredAttackers.ToList();
                    updatedDeclaredAttackers.Remove(card);
                    context.DeclaredAttackers = updatedDeclaredAttackers.ToIndexable();
                }
            }
        }

        [BehaviorModel("月光", typeof(Passive_WarriorUnblockedable))]
        public class ModelType : BehaviorModel
        { }
    }
}
