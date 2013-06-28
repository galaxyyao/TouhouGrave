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

        private ValueModifier m_attackModifier;

        CommandResult ILocalPrerequisiteTrigger<Commands.CastSpell>.RunLocalPrerequisite(Commands.CastSpell command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                "Select a card to cast Reckless on.");
            return CommandResult.Pass;
        }

        void ICastableSpell.RunSpell(Commands.CastSpell command)
        {
            var target = Game.GetTargets(this)[0];
            var warrior = target.Behaviors.Get<Warrior>();
            if (warrior != null)
            {
                Game.QueueCommands(
                    new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackModifier }),
                    new Commands.AddBehavior(target, new RecklessnessEffect.ModelType().CreateInitialized()),
                    new Commands.DeactivateAssist(Host));
            }
        }

        protected override void OnInitialize()
        {
            m_attackModifier = new ValueModifier(ValueModifierOperator.Multiply, 2);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackModifier = (original as AssistSpell_Recklessness).m_attackModifier;
        }

        [BehaviorModel(typeof(AssistSpell_Recklessness), Category = "v0.5/Assist")]
        public class ModelType : BehaviorModel
        { }
    }
}
