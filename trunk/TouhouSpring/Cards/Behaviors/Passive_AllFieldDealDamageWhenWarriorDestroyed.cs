using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFieldDealDamageWhenWarriorDestroyed:
        BaseBehavior<Passive_AllFieldDealDamageWhenWarriorDestroyed.ModelType>,
        IEpilogTrigger<Kill>
    {
        void IEpilogTrigger<Kill>.Run(CommandContext<Kill> context)
        {
            if (context.Command.Target == Host && context.Command.EnteredGraveyard)
            {
                foreach (var card in context.Game.Players.SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;
                    var warrior = card.Behaviors.Get<Warrior>();
                    if (warrior != null)
                    {
                        throw new NotImplementedException();
                        // TODO: issue command for the following:
                        //warrior.AccumulatedDamage += Model.Damage;
                    }
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
