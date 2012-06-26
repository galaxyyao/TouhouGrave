using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class Passive_AliceDollSummon : BaseBehavior<Passive_AliceDollSummon.ModelType>,
        ITrigger<PlayerTurnStartedContext>
    {
        public void Trigger(PlayerTurnStartedContext context)
        {
            if (context.Game.InPlayerPhases && IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner)
            {
                1.Repeat(i =>
                {
                    var card = new BaseCard(Model.SummonType.Target, Host.Owner);
                    context.Game.PlayCard(card);
                });
            }
        }

        [BehaviorModel("人偶召唤", typeof(Passive_AliceDollSummon))]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
