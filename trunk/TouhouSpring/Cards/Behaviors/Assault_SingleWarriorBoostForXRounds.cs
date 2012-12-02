﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Assault_SingleWarriorBoostForXRounds :
        BaseBehavior<Assault_SingleWarriorBoostForXRounds.ModelType>,
        IPrerequisiteTrigger<PlayCard>,
        ISetupTrigger<PlayCard>,
        IEpilogTrigger<PlayCard>,
        IPlayable
    {
        private BaseCard m_castTarget;

        CommandResult IPrerequisiteTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                if (!Host.Owner.CardsOnBattlefield.Any(c => c.Behaviors.Has<Warrior>()))
                {
                    return CommandResult.Cancel("No card can be affected.");
                }
            }

            return CommandResult.Pass;
        }

        CommandResult ISetupTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                var selectedCard = new Interactions.SelectCards(
                    context.Game.OpponentController, // TODO: host's controller
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

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                if (m_castTarget == null)
                {
                    throw new InvalidOperationException("Internal error: no target is selected.");
                }

                var lasting = new LastingEffect(Model.Duration);
                if (Model.AttackBoost > 0 || Model.DefenseBoost > 0)
                {
                    var enhanceMod = new Enhance(Model.AttackBoost, Model.DefenseBoost);
                    lasting.CleanUps.Add(enhanceMod);
                    context.Game.IssueCommands(new AddBehavior
                    {
                        Target = m_castTarget,
                        Behavior = enhanceMod
                    });
                }

                context.Game.IssueCommands(new AddBehavior
                {
                    Target = m_castTarget,
                    Behavior = lasting
                });
            }
        }

        public bool IsPlayable(Game game)
        {
            return Host.Owner.CardsOnBattlefield.Any(c => c.Behaviors.Has<Warrior>());
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
