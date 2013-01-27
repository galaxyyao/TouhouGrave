﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assault_SingleWarriorBoostForXRounds :
        BaseBehavior<Assault_SingleWarriorBoostForXRounds.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.NeedTarget(this,
                    Host.Owner.CardsOnBattlefield.Where(c => c.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    "Select a card to boost its attack.");
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                if (Model.AttackBoost > 0 )
                {
                    var lasting = new LastingEffect.ModelType { Duration = Model.Duration }.Instantiate();
                    var enhanceMod = new Enhance.ModelType { AttackBoost = Model.AttackBoost }.Instantiate();
                    (lasting as LastingEffect).CleanUps.Add(enhanceMod);
                    Game.IssueCommands(
                        new Commands.AddBehavior(Game.GetTarget(this)[0], enhanceMod),
                        new Commands.AddBehavior(Game.GetTarget(this)[0], lasting));
                }
            }
        }

        [BehaviorModel(typeof(Assault_SingleWarriorBoostForXRounds), DefaultName = "鬼神")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int Duration { get; set; }
        }
    }
}
