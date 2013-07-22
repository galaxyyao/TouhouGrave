using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class HealWarrior : BaseBehavior<HealWarrior.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        Commands.ICause
    {
        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(
                this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Warrior != null),
                "选择一张卡，回复 [color:Red]" + Model.Amount + "[/color] 点体力。");
            return CommandResult.Pass;
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.HealCard(Game.GetTargets(this)[0], Model.Amount, this));
        }

        [BehaviorModel(typeof(HealWarrior), Category = "v0.5/Spell", DefaultName = "治疗（单体）")]
        public class ModelType : BehaviorModel
        {
            public int Amount { get; set; }
        }
    }
}
