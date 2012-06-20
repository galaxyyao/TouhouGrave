﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class AliceDollSummon : BaseBehavior<AliceDollSummon.ModelType>,
        ITrigger<PlayerTurnStartedContext>
	{
        public void Trigger(PlayerTurnStartedContext context)
        {
            if (context.Game.InPlayerPhases && IsOnBattlefield)
            {
                1.Repeat(i =>
                {
                    var card = new BaseCard(Model.SummonType.Target, Host.Owner);
                    context.Game.PlayCard(card);
                });
            }
        }

        [BehaviorModel("人偶召唤", typeof(AliceDollSummon))]
		public class ModelType : BehaviorModel
		{
            public CardModelReference SummonType { get; set; }
            public int ManaCost { get; set; }
            public int Amount { get; set; }
		}
	}
}
