using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Assist : BaseBehavior<Assist.ModelType>
    {
        [BehaviorModel(typeof(Assist), Category = "Core", Description = "The card is served as the assistant to hero.")]
        public class ModelType : BehaviorModel
        { }
    }
}
