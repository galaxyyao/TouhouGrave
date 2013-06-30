using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , ILocalEpilogTrigger<Commands.IMoveCard>
        , IGlobalEpilogTrigger<Commands.StartPhase>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        public void RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.ToZoneType == ZoneType.OnBattlefield
                && command.FromZoneType != ZoneType.OnBattlefield
                && Host.Warrior != null)
            {
                Game.QueueCommands(
                    new Commands.AddBehavior(Host, new Effect.ModelType().CreateInitialized()),
                    new Commands.SendBehaviorMessage(Host.Warrior, "GoCoolingDown", null));
            }
        }

        public void RunGlobalEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Cleanup"
                && Game.ActingPlayer == Host.Owner
                && Host.Warrior != null
                && Host.IsOnBattlefield
                && Host.Behaviors.Has<Effect>())
            {
                Game.QueueCommands(
                    new Commands.RemoveBehavior(Host, Host.Behaviors.Get<Effect>()),
                    new Commands.SendBehaviorMessage(Host.Warrior, "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
