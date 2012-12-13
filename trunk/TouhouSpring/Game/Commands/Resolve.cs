﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    internal class Resolve : BaseCommand
    {
        internal override void ValidateOnIssue()
        {
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            foreach (var card in
                Game.Players.SelectMany(player => player.CardsOnBattlefield)
                            .Where(card => card.Behaviors.Has<Behaviors.Warrior>()))
            {
                var warrior = card.Behaviors.Get<Behaviors.Warrior>();
                if (warrior.Defense <= warrior.AccumulatedDamage)
                {
                    Game.IssueCommands(new Commands.Kill(card, null));
                }
            }
        }
    }
}