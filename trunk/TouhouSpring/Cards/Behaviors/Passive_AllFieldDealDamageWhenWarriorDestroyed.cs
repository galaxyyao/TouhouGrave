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
        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
        {
            if (command.Target == Host && command.EnteredGraveyard)
            {
                foreach (var card in command.Game.Players.SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Behaviors.Has<Hero>())
                        continue;
                    if (!card.Behaviors.Has<Warrior>())
                        continue;

                    command.Game.IssueCommands(new Commands.DealDamageToCard(card, this, Model.Damage));
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
