using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class Summon_PostPlay : BaseBehavior<Summon_PostPlay.ModelType>, ITrigger<PostCardPlayedContext>
    {
        public void Trigger(PostCardPlayedContext context)
        {
            if (IsOnBattlefield && Host == context.CardPlayed)
            {
                Model.Amount.Repeat(i =>
                {
                    context.Game.PlayCard(new BaseCard(Model.SummonType.Target, Host.Owner));
                });
            }
        }

        [BehaviorModel("Summon (PostPlay)", typeof(Summon_PostPlay))]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
            public int Amount { get; set; }
        }
    }
}
