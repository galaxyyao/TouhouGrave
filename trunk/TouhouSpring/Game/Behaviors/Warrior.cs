using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Warrior : BaseBehavior<Warrior.ModelType>
    {
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

        protected override void OnInitialize()
        {
            Attack = new IntegerEx(Model.Attack);
            Defense = new IntegerEx(Model.Defense);
            Equipments = new List<BaseCard>();
        }

        [BehaviorModel("Warrior", typeof(Warrior), Description = "The card is capable of being engaged into combats.")]
        public class ModelType : BehaviorModel
        {
            public int Attack { get; set; }
            public int Defense { get; set; }
        }
    }
}
