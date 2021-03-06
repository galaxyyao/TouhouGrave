﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_VampireNLife :
        BaseBehavior<Passive_VampireNLife.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (Host.Warrior != null
                && command.Cause == Host.Warrior
                && Host.IsOnBattlefield
                && Game.ActingPlayer == Host.Owner)
            {
                Game.QueueCommands(new Commands.HealCard(Host, Model.LifeToHeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_VampireNLife), Category = "v0.5/Passive", DefaultName = "吸血")]
        public class ModelType : BehaviorModel
        {
            public int LifeToHeal
            {
                get; set;
            }
        }
    }
}
