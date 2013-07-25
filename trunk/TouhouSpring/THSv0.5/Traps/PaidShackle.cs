using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Traps
{
    public sealed class PaidShackle : BaseBehavior<PaidShackle.ModelType>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        IGlobalEpilogTrigger<Commands.StartPhase>,
        Commands.ICause
    {
        private CardInstance m_trappingCard;
        private IBehavior m_trappingBehavior;

        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (m_trappingCard != null
                && command.Subject == m_trappingCard
                && command.ToZoneType != ZoneType.OnBattlefield)
            {
                Game.QueueCommands(new Commands.MoveCard(Host, SystemZone.Graveyard));
            }
            else if (m_trappingCard == null
                     && command.Subject != null
                     && command.Subject.Owner != Host.Owner
                     && command.FromZoneType != ZoneType.OnBattlefield
                     && command.ToZoneType == ZoneType.OnBattlefield
                     && command.Subject.Warrior != null
                     && command.Subject.Warrior.Life >= Model.TriggeringWarriorLife)
            {
                m_trappingCard = command.Subject;
                m_trappingBehavior = Model.Effect.CreateInitialized();
                Game.QueueCommands(new Commands.AddBehavior(command.Subject, m_trappingBehavior));
            }
        }

        void IGlobalEpilogTrigger<Commands.StartPhase>.RunGlobalEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Main"
                && Game.ActingPlayer == Host.Owner
                && m_trappingCard != null)
            {
                var manaCost = Host.Owner.CalculateFinalManaSubtract(Model.MaintainManaCost);
                if (Host.Owner.Mana < manaCost
                    || new Interactions.MessageBox(Host.Owner,
                                                   "支付 [color:Red]" + manaCost.ToString() + "[/color] 灵力维持枷锁？",
                                                   Interactions.MessageBoxButtons.Yes | Interactions.MessageBoxButtons.No)
                       .Run() == Interactions.MessageBoxButtons.No)
                {
                    Game.QueueCommands(
                        new Commands.RemoveBehavior(m_trappingCard, m_trappingBehavior),
                        new Commands.MoveCard(Host, SystemZone.Graveyard));
                }
                else
                {
                    Game.QueueCommands(new Commands.SubtractPlayerMana(Host.Owner, manaCost, true, this));
                }
            }
        }

        protected override void OnInitialize()
        {
            m_trappingCard = null;
            m_trappingBehavior = null;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origBhv = original as PaidShackle;
            if (origBhv.m_trappingCard != null)
            {
                m_trappingCard = Game.FindCard(origBhv.m_trappingCard.Guid, origBhv.m_trappingCard.Zone, origBhv.m_trappingCard.Owner.Index);
                m_trappingBehavior = m_trappingCard.Behaviors[origBhv.m_trappingCard.Behaviors.IndexOf(origBhv.m_trappingBehavior)];
            }
            else
            {
                m_trappingCard = null;
                m_trappingBehavior = null;
            }
        }

        [BehaviorModel(typeof(PaidShackle), Category = "v0.5/Traps", DefaultName = "枷锁（陷阱）")]
        public class ModelType : BehaviorModel
        {
            public int TriggeringWarriorLife { get; set; }
            public int MaintainManaCost { get; set; }
            public IBehaviorModel Effect { get; set; }

            public ModelType()
            {
                TriggeringWarriorLife = 4;
                MaintainManaCost = 2;
                Effect = new Effects.Shackle.ModelType();
            }
        }
    }
}
