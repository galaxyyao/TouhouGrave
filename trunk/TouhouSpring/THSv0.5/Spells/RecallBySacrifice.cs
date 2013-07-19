using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class RecallBySacrifice : BaseBehavior<RecallBySacrifice.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(
                this,
                Host.Owner.CardsSacrificed.Where(card => card.Behaviors.Has<ManaCost>()),
                "从灵力区中选择一张卡送入冥界。");
            return CommandResult.Pass;
        }

        // TODO: should cancel the whole response if user cancels at the second step
        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            var selectedTarget = Game.GetTargets(this)[0];
            var manaCost = selectedTarget.Behaviors.Get<ManaCost>();
            var selectedDead = new Interactions.SelectCardModel(Host.Owner,
                Host.Owner.Graveyard.Filter(card =>
                {
                    var mana = card.Behaviors.FirstOrDefault(bm => bm is ManaCost.ModelType);
                    return mana != null && (mana as ManaCost.ModelType).Cost <= manaCost.Cost;
                }),
                "从墓地选择一张卡加入手牌。").Run();

            if (selectedDead != null)
            {
                Game.QueueCommands(
                    new Commands.MoveCard(selectedTarget, SystemZone.Graveyard),
                    new Commands.ReviveMove(Host.Owner, selectedDead, SystemZone.Graveyard, SystemZone.Hand));
            }
        }

        [BehaviorModel(typeof(RecallBySacrifice), Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel
        { }
    }
}
