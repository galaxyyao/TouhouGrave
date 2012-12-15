using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class LastingEffect : SimpleBehavior<LastingEffect>,
        IEpilogTrigger<Commands.StartTurn>
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

        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (Host.IsOnBattlefield && Game.ActingPlayer == Host.Owner && --Duration == 0)
            {
                CleanUps.ForEach(bhv => Game.IssueCommands(new Commands.RemoveBehavior(Host, bhv)));
            }
        }
    }
}
