using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AliceDollSummonByMana : BaseBehavior<Passive_AliceDollSummonByMana.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.Subject == Host)
            {
                Game.NeedMana(1);
                if (Host.Owner.CalculateFinalManaSubtract(1) == 0)
                {
                    return CommandResult.Cancel();
                }
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.Subject == Host)
            {
                var remainingMana = Host.Owner.Mana + 1;
                var dollSummonCost = Host.Owner.CalculateFinalManaSubtract(1);
                var maxSummon = remainingMana / dollSummonCost;
                var chosenNumber = new Interactions.SelectNumber(Host.Owner, 1, maxSummon, "召唤人偶的数量？").Run();
                if (chosenNumber != null)
                {
                    chosenNumber.Value.Repeat(i => Game.QueueCommands(
                        new Commands.SummonMove(Model.SummonType.Target, Host.Owner, command.ToZone)));
                    var extraMana = dollSummonCost * chosenNumber.Value - 1;
                    if (extraMana != 0)
                    {
                        Game.QueueCommands(new Commands.SubtractPlayerMana(Host.Owner, extraMana, true, this));
                    }
                }
                else
                {
                    Game.QueueCommands(new Commands.AddPlayerMana(Host.Owner, 1, true, this));
                }
            }
        }

        [BehaviorModel(typeof(Passive_AliceDollSummonByMana), DefaultName = "人偶召唤（灵力）", Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
