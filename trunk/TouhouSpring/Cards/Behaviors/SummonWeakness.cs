using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , IEpilogTrigger<Commands.PlayCard>
        , IEpilogTrigger<Commands.StartPhase>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && Host.Behaviors.Has<Warrior>())
            {
                Game.IssueCommands(
                    new Commands.AddBehavior(Host, new Effect()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
            }
        }

        void IEpilogTrigger<Commands.StartPhase>.Run(Commands.StartPhase command)
        {
            if (command.PhaseName == "Cleanup"
                && Game.ActingPlayer == Host.Owner
                && Host.IsOnBattlefield
                && Host.Behaviors.Has<Warrior>()
                && Host.Behaviors.Has<Effect>())
            {
                Game.IssueCommands(
                    new Commands.RemoveBehavior(Host, Host.Behaviors.Get<Effect>()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
