using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class Banshee : BaseBehavior<Banshee.ModelType>,
        IGlobalEpilogTrigger<Commands.StartPhase>,
        Commands.ICause
    {
        void IGlobalEpilogTrigger<Commands.StartPhase>.RunGlobalEpilog(Commands.StartPhase command)
        {
            if (Game.ActingPlayer == Host.Owner
                && Host.IsActivatedAssist
                && command.PhaseName == "Main")
            {
                Game.QueueCommands(
                    new Commands.SubtractPlayerLife(Host.Owner, Model.SelfDamage, this),
                    new Commands.SummonMove(Model.Banshee.Value, Host.Owner, SystemZone.Battlefield));
            }
        }

        [BehaviorModel(typeof(Banshee), Category = "v0.5/Assist", DefaultName = "怨灵泉涌")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference Banshee { get; set; }
            public int SelfDamage { get; set; }
        }
    }
}
