using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AllFieldDealDamageWhenWarriorDestroyed :
        BaseBehavior<Passive_AllFieldDealDamageWhenWarriorDestroyed.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.KillMove<Commands.Battlefield>>
    {
        public void RunEpilog(Commands.KillMove<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                foreach (var card in Game.Players.SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Behaviors.Has<Hero>())
                        continue;
                    if (!card.Behaviors.Has<Warrior>())
                        continue;

                    Game.QueueCommands(new Commands.DealDamageToCard(card, Model.Damage, this));
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
