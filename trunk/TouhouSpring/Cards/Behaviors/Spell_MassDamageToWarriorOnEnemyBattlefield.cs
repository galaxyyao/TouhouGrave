using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Spell_MassDamageToWarriorOnEnemyBattlefield :
        BaseBehavior<Spell_MassDamageToWarriorOnEnemyBattlefield.ModelType>,
        ICastableSpell
    {
        void ICastableSpell.Run(CommandContext<Commands.CastSpell> context)
        {
            var warriors = (from card in context.Game.OpponentPlayer.CardsOnBattlefield
                            where card.Behaviors.Has<Warrior>() select card).ToArray();

            foreach (var warrior in warriors)
            {
                context.Game.IssueCommands(new Commands.DealDamageToCard
                {
                    Target = warrior,
                    Cause = this,
                    DamageToDeal = Model.Damage
                });
            }
        }

        [BehaviorModel(typeof(Spell_MassDamageToWarriorOnEnemyBattlefield), DefaultName = "Master Spark")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
