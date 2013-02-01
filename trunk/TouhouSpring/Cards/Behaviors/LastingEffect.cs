using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class LastingEffect : BaseBehavior<LastingEffect.ModelType>,
        IEpilogTrigger<Commands.StartPhase>
    {
        private int m_duration;

        /// <summary>
        /// Other effect that will be removed together with lastingeffect
        /// </summary>
        public List<IBehavior> CleanUps
        {
            get; private set;
        }

        public LastingEffect()
        {
            CleanUps = new List<IBehavior>();
        }

        public void RunEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Upkeep"
                && Game.ActingPlayer == Host.Owner 
                && Host.IsOnBattlefield
                && --m_duration == 0)
            {
                CleanUps.ForEach(bhv => Game.IssueCommands(new Commands.RemoveBehavior(Host, bhv)));
            }
        }

        protected override void OnInitialize()
        {
            if (Model.Duration <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration should be a positive integer.");
            }

            m_duration = Model.Duration;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var lastingEffect = original as LastingEffect;
            m_duration = lastingEffect.m_duration;
            for (int i = 0; i < lastingEffect.CleanUps.Count; ++i)
            {
                var index = lastingEffect.Host.Behaviors.IndexOf(lastingEffect.CleanUps[i]);
                CleanUps.Add(Host.Behaviors[index]);
            }
        }

        public class ModelType : BehaviorModel
        {
            public int Duration { get; set; }

            public override Type GetBehaviorType()
            {
                return typeof(LastingEffect);
            }
        }
    }
}
