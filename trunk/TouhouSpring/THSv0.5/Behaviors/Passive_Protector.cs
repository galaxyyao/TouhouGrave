using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Protector :
        BaseBehavior<Passive_Protector.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.Kill>
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && Host.Owner.CardsOnBattlefield.Any(card => card.Behaviors.Has<Behaviors.ProtectedCard>()))
                return CommandResult.Cancel();
            return CommandResult.Pass;
        }

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield
                    .Where(card => card.Behaviors.Has<Warrior>())
                    .Where(card => !card.Behaviors.Has<Behaviors.ProtectedCard>())
                    .Where(card => !card.Behaviors.Has<Passive_Protector>()))
                {
                    var protectedCard = new ProtectedCard();
                    Game.IssueCommands(new Commands.AddBehavior(card, protectedCard));
                }
            }
            if (Host.IsOnBattlefield
                && command.CardToPlay.Behaviors.Has<Warrior>()
                && !command.CardToPlay.Behaviors.Has<Passive_Protector>())
            {
                var protectedCard = new ProtectedCard();
                Game.IssueCommands(new Commands.AddBehavior(command.CardToPlay, protectedCard));
            }
        }

        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
        {
            if (command.Target == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield
                   .Where(card => card.Behaviors.Has<Warrior>())
                   .Where(card => card.Behaviors.Has<Behaviors.ProtectedCard>()))
                {
                    var protectedCardBhv = card.Behaviors.Get<ProtectedCard>();
                    Game.IssueCommands(new Commands.RemoveBehavior(card, protectedCardBhv));
                }

            }
        }

        [BehaviorModel(typeof(Passive_Protector), DefaultName = "护卫")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
