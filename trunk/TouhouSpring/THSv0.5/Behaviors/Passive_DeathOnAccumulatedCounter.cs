using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeathOnAccumulatedCounter : BaseBehavior<Passive_DeathOnAccumulatedCounter.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.EndTurn>,
        IEpilogTrigger<Commands.IMoveCard>
    {
        public class SakuraCounter : ICounter
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$sakura"; } }
            public string Text { get { return "樱花"; } }

            public static SakuraCounter Singleton = new SakuraCounter();
        }

        public void RunEpilog(Commands.EndTurn command)
        {
            if (command.Player == Host.Owner
                && Host.IsOnBattlefield)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner).SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.GetCounterCount<SakuraCounter>() == Model.NumCounters - 1)
                    {
                        Game.QueueCommands(new Commands.KillMove(card, this));
                    }
                    else
                    {
                        Game.QueueCommands(new Commands.AddCounter(card, SakuraCounter.Singleton));
                    }
                }
            }
        }

        public void RunEpilog(Commands.IMoveCard command)
        {
            if (command.Subject == Host
                && command.FromZoneType == ZoneType.OnBattlefield
                && command.ToZoneType != ZoneType.OnBattlefield)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner).SelectMany(player => player.CardsOnBattlefield))
                {
                    var numCounters = card.GetCounterCount<SakuraCounter>();
                    if (numCounters > 0)
                    {
                        Game.QueueCommands(new Commands.RemoveCounter(card, SakuraCounter.Singleton, numCounters));
                    }
                }
            }
        }

        [BehaviorModel(typeof(Passive_DeathOnAccumulatedCounter), Category = "v0.5/Passive", DefaultName = "诱亡")]
        public class ModelType : BehaviorModel
        {
            public int NumCounters { get; set; }
        }
    }
}
