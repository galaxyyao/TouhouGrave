using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class HealPlayer : BaseBehavior<HealPlayer.ModelType>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        Commands.ICause
    {
        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.AddPlayerLife(Host.Owner, Model.Amount, this));
        }

        [BehaviorModel(typeof(HealPlayer), Category = "v0.5/Spell", DefaultName = "治疗（玩家）")]
        public class ModelType : BehaviorModel
        {
            public int Amount { get; set; }
        }
    }
}
