using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AliceDollSummonByMana : BaseBehavior<Passive_AliceDollSummonByMana.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                var remainingMana = Game.GetRemainingMana(Host.Owner);
                var dollSummonCost = Host.Owner.CalculateFinalManaDelta(1);
                if (dollSummonCost != 0)
                {
                    var maxSummon = remainingMana / dollSummonCost;
                    var chosenNumber = new Interactions.SelectNumber(Host.Owner, 1, maxSummon, "召唤人偶的数量？").Run();
                    if (chosenNumber != null)
                    {
                        chosenNumber.Value.Repeat(i => Game.IssueCommands(
                            new Commands.Summon(Model.SummonType.Target, Host.Owner)));
                    }
                    Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -1 * chosenNumber.Value, this));
                }
            }
        }

        [BehaviorModel(DefaultName = "人偶召唤（灵力）", Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel<Passive_AliceDollSummonByMana>
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
