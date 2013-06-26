using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_PhantomAttack : BaseBehavior<Passive_PhantomAttack.ModelType>,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.SubtractPlayerLife>,
        IGlobalEpilogTrigger<Commands.StartPhase>,
        Commands.ICause
    {
        private bool m_triggered;

        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Target.Owner != Host.Owner
                && !m_triggered)
            {
                m_triggered = true;
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Target.Owner, command.DamageToDeal, true, this));
            }
        }

        void IGlobalEpilogTrigger<Commands.SubtractPlayerLife>.RunGlobalEpilog(Commands.SubtractPlayerLife command)
        {
            if (command.Player != Host.Owner
                && !m_triggered
                && !(command.Cause is Passive_PhantomAttack)
                && !(command.Cause is GameResource))
            {
                m_triggered = true;
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Player, command.FinalAmount, true, this));
            }
        }

        void IGlobalEpilogTrigger<Commands.StartPhase>.RunGlobalEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Main" && Game.ActingPlayer == Host.Owner)
            {
                m_triggered = false;
            }
        }

        protected override void OnInitialize()
        {
            m_triggered = false;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_triggered = (original as Passive_PhantomAttack).m_triggered;
        }

        [BehaviorModel(typeof(Passive_PhantomAttack), Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel
        { }
    }
}
