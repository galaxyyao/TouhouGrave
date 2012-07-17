using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Hero : BaseBehavior<Hero.ModelType>,
        ITrigger<Triggers.AttackPhaseStartedContext>
    {
        public void Trigger(Triggers.AttackPhaseStartedContext context)
        {
            if (context.Game.Round == 1)
            {
                Host.Behaviors.Get<Warrior>().State = WarriorState.CoolingDown;
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
