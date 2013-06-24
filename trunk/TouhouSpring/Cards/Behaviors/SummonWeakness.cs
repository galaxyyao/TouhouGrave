using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , IEpilogTrigger<Commands.IMoveCard>
        , IEpilogTrigger<Commands.StartPhase>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        public void RunEpilog(Commands.IMoveCard command)
        {
            if (command.Subject == Host
                && command.ToZoneType == ZoneType.OnBattlefield
                && command.FromZoneType != ZoneType.OnBattlefield
                && Host.Behaviors.Has<Warrior>())
            {
                Game.QueueCommands(
                    new Commands.AddBehavior(Host, new Effect.ModelType().CreateInitialized()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
            }
        }

        public void RunEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Cleanup"
                && Game.ActingPlayer == Host.Owner
                && Host.IsOnBattlefield
                && Host.Behaviors.Has<Warrior>()
                && Host.Behaviors.Has<Effect>())
            {
                Game.QueueCommands(
                    new Commands.RemoveBehavior(Host, Host.Behaviors.Get<Effect>()),
                    new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
