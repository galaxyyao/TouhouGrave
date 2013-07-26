using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaPoolAdd1 : BaseBehavior<ManaPoolAdd1.ModelType>,
        IUndefendable,
        IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$Plus1"; } }
        public string Text { get { return "扩容1"; } }

        [BehaviorModel(typeof(ManaPoolAdd1), Category = "Core", DefaultName = "扩容1（图标）")]
        public class ModelType : BehaviorModel { }
    }

    public sealed class ManaPoolSubtract1 : BaseBehavior<ManaPoolSubtract1.ModelType>,
    IUndefendable,
    IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$Minus1"; } }
        public string Text { get { return "减容1"; } }

        [BehaviorModel(typeof(ManaPoolSubtract1), Category = "Core", DefaultName = "减容1（图标）")]
        public class ModelType : BehaviorModel { }
    }

    public sealed class ManaPoolKeep : BaseBehavior<ManaPoolKeep.ModelType>,
    IUndefendable,
    IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$Minus0"; } }
        public string Text { get { return "容量不变"; } }

        [BehaviorModel(typeof(ManaPoolKeep), Category = "Core", DefaultName = "容量不变（图标）")]
        public class ModelType : BehaviorModel { }
    }
}
