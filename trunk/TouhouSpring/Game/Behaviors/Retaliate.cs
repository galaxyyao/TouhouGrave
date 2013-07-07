using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Retaliate : BaseBehavior<Retaliate.ModelType>, Commands.ICause,
        ILocalEpilogTrigger<Commands.DealDamageToCard>
    {
        void ILocalEpilogTrigger<Commands.DealDamageToCard>.RunLocalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && Host.Warrior != null
                && command.Cause != Host.Warrior
                && !Host.Behaviors.Has<IUnretaliatable>())
            {
                Game.QueueCommands(new Commands.DealDamageToCard(
                    (command.Cause as Warrior).Host,
                    Host.Warrior.Attack,
                    this));
            }
        }

        [BehaviorModel(typeof(Retaliate), Category = "Core", DefaultName = "反击")]
        public class ModelType : BehaviorModel
        { }
    }

    public interface IUnretaliatable : IBehavior { }

    public sealed class Unretaliatable : BaseBehavior<Unretaliatable.ModelType>, IUnretaliatable
    {
        [BehaviorModel(typeof(Unretaliatable), Category = "Core", DefaultName = "不能反击")]
        public class ModelType : BehaviorModel
        { }
    }
}
