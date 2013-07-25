using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class Jail : BaseBehavior<Jail.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        ILocalEpilogTrigger<Commands.IMoveCard>
    {
        private CardInstance m_prisoner;
        private IBehavior m_effectBhv;

        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(
                this,
                Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield),
                "Select a card to jail.");
            return CommandResult.Pass;
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            m_prisoner = Game.GetTargets(this)[0];
            m_effectBhv = Model.JailEffect.CreateInitialized();
            Game.QueueCommands(new Commands.AddBehavior(m_prisoner, m_effectBhv));
        }

        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.Subject == m_prisoner
                && m_prisoner != null
                && command.FromZoneType == ZoneType.OnBattlefield
                && command.ToZoneType != ZoneType.OnBattlefield)
            {
                m_prisoner = null;
                m_effectBhv = null;
            }
        }

        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType == ZoneType.OnBattlefield
                && command.ToZoneType != ZoneType.OnBattlefield
                && m_prisoner != null)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(m_prisoner, m_effectBhv));
                m_prisoner = null;
                m_effectBhv = null;
            }
        }

        protected override void OnInitialize()
        {
            m_prisoner = null;
            m_effectBhv = null;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origJail = original as Jail;
            if (origJail.m_prisoner != null)
            {
                m_prisoner = Game.FindCard(origJail.m_prisoner.Guid, origJail.m_prisoner.Zone, origJail.m_prisoner.Owner.Index);
                m_effectBhv = m_prisoner.Behaviors[origJail.m_prisoner.Behaviors.IndexOf(origJail.m_effectBhv)];
            }
            else
            {
                m_prisoner = null;
                m_effectBhv = null;
            }
        }

        [BehaviorModel(typeof(Jail), Category = "v0.5/Passive", DefaultName = "监禁")]
        public class ModelType : BehaviorModel
        {
            public IBehaviorModel JailEffect { get; set; }

            public ModelType()
            {
                JailEffect = new Effects.Shackle.ModelType();
            }
        }
    }
}
