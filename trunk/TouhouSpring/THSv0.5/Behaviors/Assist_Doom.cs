using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_Doom : BaseBehavior<Assist_Doom.ModelType>,
        IGlobalPrologTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.EndPhase>,
        Commands.ICause
    {
        class DoomCounter : ICounter
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$Felfire"; } }
            public string Text { get { return "厄运"; } }

            public static DoomCounter Singleton = new DoomCounter();
        }

        void IGlobalPrologTrigger<Commands.DealDamageToCard>.RunGlobalProlog(Commands.DealDamageToCard command)
        {
            if (command.Target.Owner == Host.Owner
                && command.DamageToDeal > 0)
            {
                Game.QueueCommands(new Commands.AddCounter(Host, DoomCounter.Singleton, command.DamageToDeal));
                command.PatchDamageToDeal(0);
            }
        }

        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (Game.ActingPlayer == Host.Owner && command.PreviousPhase == "Main")
            {
                int numCounters = Host.GetCounterCount<DoomCounter>();
                if (numCounters > 1)
                {
                    Game.QueueCommands(new Commands.RemoveCounter(Host, DoomCounter.Singleton, numCounters));
                }
                if (numCounters >= Model.DamageThreshold)
                {
                    Game.QueueCommands(new Commands.SubtractPlayerLife(Host.Owner, numCounters, this));
                }
            }
        }

        [BehaviorModel(typeof(Assist_Doom), Category = "v0.5/Assist")]
        public class ModelType : BehaviorModel
        {
            [System.ComponentModel.Description("当移除的厄运指示物大于此数值时，对玩家造成造成伤害。")]
            public int DamageThreshold { get; set; }
        }
    }
}
