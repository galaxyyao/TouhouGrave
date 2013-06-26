using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Stealth : BaseBehavior<Passive_Stealth.ModelType>,
        ILocalEpilogTrigger<Commands.IMoveCard>,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.StartTurn>
    {
        private IBehavior m_undefendable;
        private bool m_lastTurnAttacked = false;

        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                Game.QueueCommands(new Commands.AddBehavior(Host, m_undefendable));
            }
            else if (command.FromZoneType == ZoneType.OnBattlefield
                     && command.ToZoneType != ZoneType.OnBattlefield)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(Host, m_undefendable));
            }
        }

        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && command.Cause == Host.Behaviors.Get<Warrior>())
            {
                m_lastTurnAttacked = true;
            }
        }

        void IGlobalEpilogTrigger<Commands.StartTurn>.RunGlobalEpilog(Commands.StartTurn command)
        {
            if (command.Player != Host.Owner)
            {
                if (m_lastTurnAttacked)
                {
                    Game.QueueCommands(new Commands.RemoveBehavior(Host, m_undefendable));
                }
            }
            else if (command.Player == Host.Owner)
            {
                m_lastTurnAttacked = false;
                Game.QueueCommands(new Commands.AddBehavior(Host, m_undefendable));
            }
        }

        protected override void OnInitialize()
        {
            m_undefendable = new Undefendable.ModelType().CreateInitialized();
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origStealth = original as Passive_Stealth;
            var invulnerableIndex = original.Host.Behaviors.IndexOf(origStealth.m_undefendable);
            if (invulnerableIndex != -1)
            {
                m_undefendable = Host.Behaviors[invulnerableIndex];
                Debug.Assert(m_undefendable is Undefendable);
            }
            else
            {
                m_undefendable = new Undefendable.ModelType().CreateInitialized();
            }
        }

        [BehaviorModel(typeof(Passive_Stealth), Category = "v0.5/Passive", DefaultName = "雾隐")]
        public class ModelType : BehaviorModel
        { }
    }
}
