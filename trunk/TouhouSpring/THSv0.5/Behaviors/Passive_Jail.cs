using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Jail : BaseBehavior<Passive_Jail.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        ILocalEpilogTrigger<Commands.IMoveCard>
    {
        class JailEffect : Unattackable,
            IGlobalPrologTrigger<Commands.DealDamageToCard>
        {
            void IGlobalPrologTrigger<Commands.DealDamageToCard>.RunGlobalProlog(Commands.DealDamageToCard command)
            {
                if (command.Cause is Passive_Retaliate && (command.Cause as IBehavior).Host == Host)
                {
                    command.PatchDamageToDeal(0);
                }
            }

            [BehaviorModel(typeof(JailEffect), HideFromEditor = true)]
            new public class ModelType : Unattackable.ModelType { }
        }

        private CardInstance m_prisoner;

        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(
                this,
                Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield)
                    .ToArray().ToIndexable(),
                "Select a card to jail.");
            return CommandResult.Pass;
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            m_prisoner = Game.GetTargets(this)[0];
            Game.QueueCommands(new Commands.AddBehavior(m_prisoner, new JailEffect.ModelType().CreateInitialized()));
        }

        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.Subject == m_prisoner
                && m_prisoner != null
                && command.FromZoneType == ZoneType.OnBattlefield
                && command.ToZoneType != ZoneType.OnBattlefield)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(m_prisoner, m_prisoner.Behaviors.Get<JailEffect>()));
                m_prisoner = null;
            }
        }

        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType == ZoneType.OnBattlefield
                && command.ToZoneType != ZoneType.OnBattlefield
                && m_prisoner != null)
            {
                Game.QueueCommands(new Commands.RemoveBehavior(m_prisoner, m_prisoner.Behaviors.Get<JailEffect>()));
                m_prisoner = null;
            }
        }

        protected override void OnInitialize()
        {
            m_prisoner = null;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origJail = original as Passive_Jail;
            m_prisoner = origJail.m_prisoner != null
                         ? Game.FindCard(origJail.m_prisoner.Guid, origJail.m_prisoner.Zone, origJail.Game.Players.IndexOf(origJail.m_prisoner.Owner))
                         : null;
        }

        [BehaviorModel(typeof(Passive_Jail), Category = "v0.5/Passive", DefaultName = "冻结")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
