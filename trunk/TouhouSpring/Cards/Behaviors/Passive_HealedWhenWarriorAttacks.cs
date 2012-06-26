using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_HealedWhenWarriorAttacks :
        BaseBehavior<Passive_HealedWhenWarriorAttacks.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>
    {
        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.CardDamaged.Owner != Host.Owner
                && IsOnBattlefield
                && context.Cause.Host.Behaviors.Get<Behaviors.Hero>() == null
                && context.Cause.Host.Behaviors.Get<Behaviors.Warrior>() != null)
            {
                context.Game.UpdateHealth(Host.Owner, -context.DamageDealt, Host.Owner.Hero);
            }
        }

        [BehaviorModel("吸血鬼", typeof(Passive_HealedWhenWarriorAttacks))]
        public class ModelType : BehaviorModel
        { }
    }
}
