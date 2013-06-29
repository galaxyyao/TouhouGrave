using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Dismiss : BaseBehavior<Passive_Dismiss.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        Commands.ICause
    {
        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedOptionalTargets(
                this,
                Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield),
                "Select a card to dismiss.");
            return CommandResult.Pass;
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            var targets = Game.GetTargets(this);
            if (targets.Count == 1)
            {
                Game.QueueCommands(new Commands.MoveCard(targets[0], SystemZone.Hand, this));
            }
        }

        [BehaviorModel(typeof(Passive_Dismiss), Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
