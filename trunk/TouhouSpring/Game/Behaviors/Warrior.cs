using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public enum WarriorState
    {
        StandingBy,
        CoolingDown
    }

    public class Warrior : BaseBehavior<Warrior.ModelType>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        public WarriorState State
        {
            get; internal set;
        }

        public IntegerEx Attack
        {
            get; private set;
        }

        public IntegerEx Defense
        {
            get; private set;
        }

        public int AccumulatedDamage
        {
            get; set;
        }

        public List<BaseCard> Equipments
        {
            get; private set;
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                State = WarriorState.StandingBy;
            }
        }

        protected override void OnInitialize()
        {
            State = WarriorState.StandingBy;
            Attack = new IntegerEx(Model.Attack);
            Defense = new IntegerEx(Model.Defense);
            Equipments = new List<BaseCard>();
        }

        [BehaviorModel(typeof(Warrior), Category = "Core", Description = "The card is capable of being engaged into combats.")]
        public class ModelType : BehaviorModel
        {
            public int Attack { get; set; }
            public int Defense { get; set; }
        }
    }
}
