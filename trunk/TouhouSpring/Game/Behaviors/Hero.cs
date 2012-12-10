using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Hero : BaseBehavior<Hero.ModelType>,
        IEpilogTrigger<Commands.StartAttackPhase>
    {
        void IEpilogTrigger<Commands.StartAttackPhase>.Run(Commands.StartAttackPhase command)
        {
            if (command.Game.Round == 1)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    command.Game.IssueCommands(
                        new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
                }
            }
        }

        [BehaviorModel(typeof(Hero), Category = "Core", Description = "The card is served as the main character.")]
        public class ModelType : BehaviorModel
        {
            public int Health { get; set; }
            public int Mana { get; set; }
        }
    }
}
