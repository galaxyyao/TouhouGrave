﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Sputter_NDamage:
        BaseBehavior<Passive_Sputter_NDamage.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (Host.IsOnBattlefield
                && command.Cause == Host.Behaviors.Get<Warrior>())
            {
                Game.IssueCommands(new Commands.DealDamageToPlayer(Game.ActingPlayerEnemies.First(), Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "溅射")]
        public class ModelType:BehaviorModel<Passive_Sputter_NDamage>
        {
            public int DamageToDeal
            {
                get;
                set;
            }
        }
    }
}
