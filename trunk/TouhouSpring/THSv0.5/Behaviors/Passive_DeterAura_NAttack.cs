using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeterAura_NAttack :
        BaseBehavior<Passive_DeterAura_NAttack.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.IMoveCard>,
        IGlobalEpilogTrigger<Commands.IMoveCard>
    {
        private ValueModifier m_attackMod;

        public void RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                // enter battlefield
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                        .SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Warrior != null)
                    {
                        AffectByAura(card);
                    }
                }
            }
            else if (command.FromZoneType == ZoneType.OnBattlefield
                     && command.ToZoneType != ZoneType.OnBattlefield)
            {
                // leave battlefield
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield))
                {
                    if (card.Warrior != null)
                    {
                        LeaveAura(card);
                    }
                }
            }
        }

        public void RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.Subject != Host
                && command.Subject.Owner != Host.Owner
                && command.Subject.Warrior != null
                && Host.IsOnBattlefield)
            {
                if (command.FromZoneType != ZoneType.OnBattlefield
                    && command.ToZoneType == ZoneType.OnBattlefield)
                {
                    // enter battlefield
                    AffectByAura(command.Subject);
                }
                else if (command.FromZoneType == ZoneType.OnBattlefield
                         && command.ToZoneType != ZoneType.OnBattlefield)
                {
                    // leave battlefield
                    LeaveAura(command.Subject);
                }
            }
        }

        private void AffectByAura(CardInstance card)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
        }

        private void LeaveAura(CardInstance card)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, -1);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Passive_DeterAura_NAttack).m_attackMod;
        }

        [BehaviorModel(typeof(Passive_DeterAura_NAttack), Category = "v0.5/Passive", DefaultName = "灵压")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
