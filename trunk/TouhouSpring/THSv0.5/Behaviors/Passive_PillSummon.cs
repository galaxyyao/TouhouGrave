﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_PillSummon:
        BaseBehavior<Passive_PillSummon.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.IMoveCard>
    {
        public void RunEpilog(Commands.IMoveCard command)
        {
            if (Game.ActingPlayer == Host.Owner
                && command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                Model.NumToSummon.Repeat(() =>
                {
                    Game.QueueCommands(new Commands.SummonMove(Model.SummonType.Target, Host.Owner, SystemZone.Sacrifice));
                });
            }
        }

        [BehaviorModel(typeof(Passive_PillSummon), Category = "v0.5/Passive", DefaultName = "毛玉召唤")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }

            public int NumToSummon
            {
                get; set;
            }
        }
    }
}
