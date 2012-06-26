using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_HeroPenetrate :
        BaseBehavior<Passive_HeroPenetrate.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>
    {
        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (IsOnBattlefield
                && context.Cause.Host == context.Game.PlayerPlayer.Hero.Host)
            {
                var damagedWarrior = context.CardDamaged.Behaviors.Get<Warrior>();
                int overflow = Math.Min(Math.Max(damagedWarrior.AccumulatedDamage - damagedWarrior.Defense, 0), context.DamageDealt);
                context.Game.UpdateHealth(context.CardDamaged.Owner, -overflow, this);
            }
        }

        [BehaviorModel("式神化猫", typeof(Passive_HeroPenetrate))]
        public class ModelType : BehaviorModel
        { }
    }
}
