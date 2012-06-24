using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Support : BaseBehavior<Support.ModelType>
    {
        [BehaviorModel("Support", typeof(Support), Description = "The card is served as the support character.")]
        public class ModelType : BehaviorModel
        { }
    }
}
