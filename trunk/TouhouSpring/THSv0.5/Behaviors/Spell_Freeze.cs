﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_Freeze : BaseBehavior<Spell_Freeze.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && !Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield)
                    .Any())
            {
                return CommandResult.Cancel("No card can be affected.");
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                                                 .SelectMany(player => player.CardsOnBattlefield))
                {
                    var lasting = new LastingEffect.ModelType { Duration = 2 }.Instantiate();
                    var neutralize = new Neutralize.ModelType().Instantiate();
                    (lasting as LastingEffect).CleanUps.Add(neutralize);
                    Game.IssueCommands(
                        new Commands.AddBehavior(card, neutralize),
                        new Commands.AddBehavior(card, lasting));
                }
            }
        }

        [BehaviorModel(typeof(Spell_Freeze), Category = "v0.5/Spell", DefaultName = "冰冻")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
