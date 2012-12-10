using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , IEpilogTrigger<Commands.PlayCard>
        , IEpilogTrigger<Commands.EndTurn>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && Host.Behaviors.Has<Warrior>())
            {
                command.Game.IssueCommands(
                    new Commands.AddBehavior(Host, new Effect()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
            }
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(Commands.EndTurn command)
        {
            if (IsOnBattlefield
                && command.Game.PlayerPlayer == Host.Owner
                && Host.Behaviors.Has<Warrior>()
                && Host.Behaviors.Has<Effect>())
            {
                command.Game.IssueCommands(
                    new Commands.RemoveBehavior(Host, Host.Behaviors.Get<Effect>()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
