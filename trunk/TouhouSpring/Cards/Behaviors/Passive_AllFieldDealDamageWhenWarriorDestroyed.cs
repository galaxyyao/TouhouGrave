using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFieldDealDamageWhenWarriorDestroyed:
        BaseBehavior<Passive_AllFieldDealDamageWhenWarriorDestroyed.ModelType>,
        IEpilogTrigger<Commands.Kill>
    {
        void IEpilogTrigger<Commands.Kill>.Run(CommandContext<Commands.Kill> context)
        {
            if (context.Command.Target == Host && context.Command.EnteredGraveyard)
            {
                foreach (var card in context.Game.Players.SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Behaviors.Has<Hero>())
                        continue;
                    if (!card.Behaviors.Has<Warrior>())
                        continue;

                    context.Game.IssueCommands(new Commands.DealDamageToCard
                    {
                        Target = card,
                        Cause = this,
                        DamageToDeal = Model.Damage
                    });
                }
            }
        }

        [BehaviorModel(typeof(Passive_AllFieldDealDamageWhenWarriorDestroyed), DefaultName = "凤凰")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
