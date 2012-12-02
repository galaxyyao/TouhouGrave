using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

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
                context.Game.IssueCommands(new DealDamageToPlayer
                {
                    Target = context.CardDamaged.Owner,
                    DamageToDeal = overflow,
                    Cause = this
                });
            }
        }

        [BehaviorModel(typeof(Passive_HeroPenetrate), DefaultName = "式神化猫")]
        public class ModelType : BehaviorModel
        { }
    }
}
