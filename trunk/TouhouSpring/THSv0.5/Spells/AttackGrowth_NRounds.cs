using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class AttackGrowth_NRounds : BaseBehavior<AttackGrowth_NRounds.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        private class Effect : Utilities.LastingEffect.EffectUponPhaseStart<Effect.ModelType>, IStatusEffect
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$BTNRegeneration"; } }
            public string Text { get { return "增强\n攻击力增长。"; } }

            protected override void OnEffectGone()
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Warrior, WarriorMessage.RemoveAttackModifier, Model.ValueModifier));
            }

            [BehaviorModel(typeof(Effect), HideFromEditor = true)]
            public class ModelType : Utilities.LastingEffect.ModelType
            {
                public ValueModifier ValueModifier;
            }
        }

        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(c => c.Warrior != null),
                "Select a card to boost its attack.");
            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            var effect = new Effect.ModelType
            {
                PhaseName = "Upkeep",
                LastingTurns = Model.Duration * 2,
                ValueModifier = Model.AttackModifier
            }.CreateInitialized();
            var target = Game.GetTargets(this)[0];
            Game.QueueCommands(
                new Commands.AddBehavior(target, effect),
                new Commands.SendBehaviorMessage(target.Warrior, WarriorMessage.AddAttackModifier, Model.AttackModifier));
        }

        [BehaviorModel(typeof(AttackGrowth_NRounds), Category = "v0.5/Spell", DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
            public ValueModifierOperator AttackModifierOperator
            {
                get { return AttackModifier.Operator; }
                set { AttackModifier = new ValueModifier(value, AttackModifierAmount, false); }
            }

            public int AttackModifierAmount
            {
                get { return AttackModifier.Amount; }
                set { AttackModifier = new ValueModifier(AttackModifierOperator, value, false); }
            }

            [System.ComponentModel.Browsable(false)]
            public ValueModifier AttackModifier { get; private set; }

            public int Duration { get; set; }

            public ModelType()
            {
                AttackModifier = new ValueModifier(ValueModifierOperator.Add, 1);
            }
        }
    }
}
