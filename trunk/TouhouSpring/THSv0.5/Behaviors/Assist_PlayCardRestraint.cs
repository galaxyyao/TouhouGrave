using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_PlayCardRestraint : BaseBehavior<Assist_PlayCardRestraint.ModelType>,
        IGlobalPrerequisiteTrigger<Commands.PlayCard>,
        IGlobalEpilogTrigger<Commands.PlayCard>,
        IGlobalEpilogTrigger<Commands.StartTurn>
    {
        // avoiding allocating array...
        private int m_counter;

        CommandResult IGlobalPrerequisiteTrigger<Commands.PlayCard>.RunGlobalPrerequisite(Commands.PlayCard command)
        {
            if (!Host.IsActivatedAssist)
            {
                return CommandResult.Pass;
            }

            if (command.Initiator == Host.Owner && (Model.Affects & ModelType.RestraintAffects.Ally) != 0
                || command.Initiator != Host.Owner && (Model.Affects & ModelType.RestraintAffects.Enemy) != 0)
            {
                switch (Model.Scheme)
                {
                    case ModelType.RestraintScheme.PerTurn:
                        if (m_counter >= Model.Amount)
                        {
                            return CommandResult.Cancel();
                        }
                        break;

                    case ModelType.RestraintScheme.Total:
                        if (command.Initiator.CardsOnBattlefield.Count >= Model.Amount)
                        {
                            return CommandResult.Cancel();
                        }
                        break;

                    default:
                        break;
                }
            }

            return CommandResult.Pass;
        }

        void IGlobalEpilogTrigger<Commands.PlayCard>.RunGlobalEpilog(Commands.PlayCard command)
        {
            ++m_counter;
        }

        void IGlobalEpilogTrigger<Commands.StartTurn>.RunGlobalEpilog(Commands.StartTurn command)
        {
            m_counter = 0;
        }

        protected override void OnInitialize()
        {
            m_counter = 0;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origBhv = original as Assist_PlayCardRestraint;
            m_counter = origBhv.m_counter;
        }

        [BehaviorModel(typeof(Assist_PlayCardRestraint), Category = "v0.5/Assist", DefaultName = "上场限制")]
        public class ModelType : BehaviorModel
        {
            public enum RestraintAffects
            {
                Ally = 0x01,
                Enemy = 0x02,
                Both = Ally | Enemy,
            }

            public enum RestraintScheme
            {
                PerTurn,
                Total
            }

            public RestraintAffects Affects { get; set; }
            public RestraintScheme Scheme { get; set; }
            public int Amount { get; set; }
        }
    }
}
