using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class LastingEffect : SimpleBehavior<LastingEffect>,
        IEpilogTrigger<Commands.StartPhase>
    {
        public int Duration
        {
            get; private set;
        }

        /// <summary>
        /// Other effect that will be removed together with lastingeffect
        /// </summary>
        public List<IBehavior> CleanUps
        {
            get; private set;
        }

        public LastingEffect(int duration)
        {
            if (duration <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration should be a positive integer.");
            }
            Duration = duration;
            CleanUps = new List<IBehavior>();
        }

        void IEpilogTrigger<Commands.StartPhase>.Run(Commands.StartPhase command)
        {
            if (command.PhaseName == "Upkeep"
                && Game.ActingPlayer == Host.Owner 
                && Host.IsOnBattlefield
                && --Duration == 0)
            {
                CleanUps.ForEach(bhv => Game.IssueCommands(new Commands.RemoveBehavior(Host, bhv)));
            }
        }
    }
}
