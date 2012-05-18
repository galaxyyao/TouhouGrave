using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Hero : BaseBehavior<Hero.ModelType>
    {
        [BehaviorModel("Hero", typeof(Hero), Description = "The card is served as the main character.")]
        public class ModelType : BehaviorModel
        {
            public int Health { get; set; }
            public int Mana { get; set; }
        }
    }
}
