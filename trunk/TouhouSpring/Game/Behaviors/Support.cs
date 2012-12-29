using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Support : BaseBehavior<Support.ModelType>,
        Commands.ICause
    {
        [BehaviorModel(typeof(Support), Category = "Core", Description = "The card is served as the support character.")]
        public class ModelType : BehaviorModel
        { }
    }
}
