using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Hero : BaseBehavior<Hero.ModelType>
    {
        public int InitialHealth
        {
            get { return Model.Health; }
        }

        [BehaviorModel(typeof(Hero), Category = "Core", Description = "The card is served as the main character.")]
        public class ModelType : BehaviorModel
        {
            public int Health { get; set; }
        }
    }
}
