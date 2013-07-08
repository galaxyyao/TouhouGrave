using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IForceAttack : IBehavior { }
    public interface IProtector : IBehavior { }
    public interface ITaunt : IBehavior { }
    public interface IUnattackable : IBehavior { }
    public interface IUndefendable : IBehavior { }
    public interface IUnselectable : IBehavior { }

    public interface ICounter
    {
        string IconUri { get; }
        string Text { get; }
    }

    public interface IStatusEffect
    {
        string IconUri { get; }
        string Text { get; }
    }

    public class ForceAttack : BaseBehavior<ForceAttack.ModelType>, IForceAttack
    {
        [BehaviorModel(typeof(ForceAttack), Category = "Core", Description = "Card must attack if it can.")]
        public class ModelType : BehaviorModel
        { }
    }

    public class Protector : BaseBehavior<Protector.ModelType>, IProtector
    {
        [BehaviorModel(typeof(Protector), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }

    public class Taunt : BaseBehavior<Taunt.ModelType>
    {
        [BehaviorModel(typeof(Taunt), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }

    public class Unattackable : BaseBehavior<Unattackable.ModelType>, IUnattackable
    {
        [BehaviorModel(typeof(Unattackable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }

    public class Undefendable : BaseBehavior<Undefendable.ModelType>, IUndefendable
    {
        [BehaviorModel(typeof(Undefendable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }

    public class Unselectable : BaseBehavior<Unselectable.ModelType>, IUnselectable
    {
        [BehaviorModel(typeof(Unselectable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
