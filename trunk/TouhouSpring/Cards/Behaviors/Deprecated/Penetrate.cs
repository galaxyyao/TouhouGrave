using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Penetrate : BaseBehavior<Penetrate.ModelType>, ITrigger<Triggers.PostCardDamagedContext>
    {
        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.Cause is Warrior && context.Cause.Host == Host)
            {
                var damagedWarrior = context.CardDamaged.Behaviors.Get<Warrior>();
                int overflow = Math.Min(Math.Max(damagedWarrior.AccumulatedDamage - damagedWarrior.Defense, 0), context.DamageDealt);
                context.Game.UpdateHealth(context.CardDamaged.Owner, -overflow, this);
            }
        }

        [BehaviorModel(typeof(Penetrate), Category = "Deprecated", DefaultName = "穿刺")]
        public class ModelType : BehaviorModel
        { }
    }
}
