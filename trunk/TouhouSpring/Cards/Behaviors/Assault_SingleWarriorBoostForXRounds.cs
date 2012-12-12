using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assault_SingleWarriorBoostForXRounds :
        BaseBehavior<Assault_SingleWarriorBoostForXRounds.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>,
        ISetupTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private BaseCard m_castTarget;

        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                if (!Host.Owner.CardsOnBattlefield.Any(c => c.Behaviors.Has<Warrior>()))
                {
                    return CommandResult.Cancel("No card can be affected.");
                }
            }

            return CommandResult.Pass;
        }

        CommandResult ISetupTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                var selectedCard = new Interactions.SelectCards(
                    command.Game.OpponentController, // TODO: host's controller
                    Host.Owner.CardsOnBattlefield.Where(c => c.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    Interactions.SelectCards.SelectMode.Single,
                    "Select a card to boost its attack and defense.").Run();

                if (selectedCard.Count == 0)
                {
                    return CommandResult.Cancel("Boost is canceled.");
                }

                m_castTarget = selectedCard[0];
            }

            return CommandResult.Pass;
        }

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                if (m_castTarget == null)
                {
                    throw new InvalidOperationException("Internal error: no target is selected.");
                }

                if (Model.AttackBoost > 0 || Model.DefenseBoost > 0)
                {
                    var lasting = new LastingEffect(Model.Duration);
                    var enhanceMod = new Enhance(Model.AttackBoost, Model.DefenseBoost);
                    lasting.CleanUps.Add(enhanceMod);
                    command.Game.IssueCommands(
                        new Commands.AddBehavior(m_castTarget, enhanceMod),
                        new Commands.AddBehavior(m_castTarget, lasting));
                }
            }
        }

        [BehaviorModel(typeof(Assault_SingleWarriorBoostForXRounds), DefaultName = "鬼神")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int DefenseBoost { get; set; }
            public int Duration { get; set; }
        }
    }
}
