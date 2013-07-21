using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Utilities
{
    public static class LastingEffect
    {
        public class BaseEffect<T> : BaseBehavior<T>
            where T : ModelType
        {
            private int m_counter;

            protected virtual void OnEffectGone() { }

            protected void DecreaseCounter()
            {
                if (--m_counter <= 0)
                {
                    Game.QueueCommands(new Commands.RemoveBehavior(Host, this));
                    OnEffectGone();
                }
            }

            protected override void OnInitialize()
            {
                m_counter = Model.LastingTurns;
            }

            protected override void OnTransferFrom(IBehavior original)
            {
                m_counter = (original as BaseEffect<T>).m_counter;
            }
        }

        public class EffectUponPhaseStart<T> : BaseEffect<T>,
            IGlobalEpilogTrigger<Commands.StartPhase>
            where T : ModelType
        {
            void IGlobalEpilogTrigger<Commands.StartPhase>.RunGlobalEpilog(Commands.StartPhase command)
            {
                if (command.PhaseName == Model.PhaseName)
                {
                    DecreaseCounter();
                }
            }
        }

        public class EffectUponPhaseEnd<T> : BaseEffect<T>,
            IGlobalEpilogTrigger<Commands.EndPhase>
            where T : ModelType
        {
            void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
            {
                if (command.PreviousPhase == Model.PhaseName)
                {
                    DecreaseCounter();
                }
            }
        }

        public class ModelType : BehaviorModel
        {
            public string PhaseName { get; set; }
            // turn is for one player; 2 turns is one round.
            public int LastingTurns { get; set; }

            public ModelType()
            {
                PhaseName = "Main";
            }
        }
    }
}
