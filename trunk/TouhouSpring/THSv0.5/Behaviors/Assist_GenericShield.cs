using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Assist_GenericShield : BaseBehavior<Assist_GenericShield.ModelType>,
        IGlobalPrologTrigger<Commands.DealDamageToCard>
    {
        void IGlobalPrologTrigger<Commands.DealDamageToCard>.RunGlobalProlog(Commands.DealDamageToCard command)
        {
            if (Host.IsActivatedAssist && command.Target.Owner == Host.Owner)
            {
                int newDamage = (command.Cause is Warrior
                                 ? Model.WarriorDamageModifier
                                 : Model.SpellDamageModifier).Process(command.DamageToDeal);
                command.PatchDamageToDeal(Math.Max(newDamage, 0));
            }
        }

        [BehaviorModel(typeof(Assist_GenericShield), Category = "v0.5/Assist", DefaultName = "护盾（通用）")]
        public class ModelType : BehaviorModel
        {
            [System.ComponentModel.Category("Damage from Warrior")]
            [System.ComponentModel.DisplayName("Operator")]
            public ValueModifierOperator WarriorDamageOperator
            {
                get { return WarriorDamageModifier.Operator; }
                set { WarriorDamageModifier = new ValueModifier(value, WarriorDamageAmount, false); }
            }

            [System.ComponentModel.Category("Damage from Warrior")]
            [System.ComponentModel.DisplayName("Amount")]
            public int WarriorDamageAmount
            {
                get { return WarriorDamageModifier.Amount; }
                set { WarriorDamageModifier = new ValueModifier(WarriorDamageOperator, value, false); }
            }

            [System.ComponentModel.Category("Damage from Spell")]
            [System.ComponentModel.DisplayName("Operator")]
            public ValueModifierOperator SpellDamageOperator
            {
                get { return SpellDamageModifier.Operator; }
                set { SpellDamageModifier = new ValueModifier(value, SpellDamageAmount, false); }
            }

            [System.ComponentModel.Category("Damage from Spell")]
            [System.ComponentModel.DisplayName("Amount")]
            public int SpellDamageAmount
            {
                get { return SpellDamageModifier.Amount; }
                set { SpellDamageModifier = new ValueModifier(SpellDamageOperator, value, false); }
            }

            [System.ComponentModel.Browsable(false)]
            public ValueModifier WarriorDamageModifier { get; private set; }
            [System.ComponentModel.Browsable(false)]
            public ValueModifier SpellDamageModifier { get; private set; }

            public ModelType()
            {
                WarriorDamageModifier = new ValueModifier(ValueModifierOperator.Add, 0);
                SpellDamageModifier = new ValueModifier(ValueModifierOperator.Add, 0);
            }
        }
    }
}
