using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class AssistSpell_Recklessness : BaseBehavior<AssistSpell_Recklessness.ModelType>,
        ILocalPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        public class RecklessnessEffect : SimpleBehavior<RecklessnessEffect>,
            IGlobalEpilogTrigger<Commands.EndPhase>, Commands.ICause
        {
            void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
            {
                if (Game.ActingPlayer == Host.Owner
                    && command.PreviousPhase == "Main")
                {
                    Game.QueueCommands(new Commands.MoveCard(Host, SystemZone.Graveyard, this));
                }
            }
        }

        CommandResult ILocalPrerequisiteTrigger<Commands.CastSpell>.RunLocalPrerequisite(Commands.CastSpell command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Warrior != null),
                "Select a card to cast Reckless on.");
            return CommandResult.Pass;
        }

        void ICastableSpell.RunSpell(Commands.CastSpell command)
        {
            var target = Game.GetTargets(this)[0];
            if (target.Warrior != null)
            {
                Game.QueueCommands(
                    new Commands.SendBehaviorMessage(target.Warrior, WarriorMessage.AddAttackModifier, Model.Modifier),
                    new Commands.AddBehavior(target, new RecklessnessEffect.ModelType().CreateInitialized()),
                    new Commands.DeactivateAssist(Host));
            }
        }

        [BehaviorModel(typeof(AssistSpell_Recklessness), Category = "v0.5/Assist")]
        public class ModelType : BehaviorModel
        {
            public ValueModifierOperator Operator
            {
                get { return Modifier.Operator; }
                set { Modifier = new ValueModifier(value, Amount, false); }
            }

            public int Amount
            {
                get { return Modifier.Amount; }
                set { Modifier = new ValueModifier(Operator, value, false); }
            }

            [System.ComponentModel.Browsable(false)]
            public ValueModifier Modifier
            {
                get; private set;
            }

            public ModelType()
            {
                Modifier = new ValueModifier(ValueModifierOperator.Multiply, 2);
            }
        }
    }
}
