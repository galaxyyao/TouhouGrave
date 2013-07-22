using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class SummonToHandOnTurnStart : BaseBehavior<SummonToHandOnTurnStart.ModelType>,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (Game.ActingPlayer == Host.Owner
                && command.PreviousPhase == "Upkeep"
                && Model.ChoiceA.Value != null
                && Model.ChoiceB.Value != null)
            {
                var chosenCard = new Interactions.SelectCardModel(
                    Host.Owner,
                    new ICardModel[] { Model.ChoiceA.Value, Model.ChoiceB.Value },
                    "选择一张卡加入手牌。").Run();
                if (chosenCard != null)
                {
                    Game.QueueCommands(new Commands.SummonMove(chosenCard, Host.Owner, SystemZone.Hand));
                }
            }
        }

        [BehaviorModel(typeof(SummonToHandOnTurnStart), Category = "v0.5/Assist", DefaultName = "红薯或地瓜")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference ChoiceA { get; set; }
            public CardModelReference ChoiceB { get; set; }
        }
    }
}
