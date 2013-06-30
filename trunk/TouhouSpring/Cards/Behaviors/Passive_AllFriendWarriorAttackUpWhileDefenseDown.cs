using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AllFriendWarriorAttackUpWhileDefenseUp :
        BaseBehavior<Passive_AllFriendWarriorAttackUpWhileDefenseUp.ModelType>,
        ILocalEpilogTrigger<Commands.IMoveCard>,
        IGlobalEpilogTrigger<Commands.IMoveCard>
    {
        private ValueModifier m_attackMod;
        private ValueModifier m_defenseMod;

        public void RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                // enter battlefield
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                            new Commands.SendBehaviorMessage(card.Warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
                    }
                }
            }
            else if (command.FromZoneType == ZoneType.OnBattlefield
                     && command.ToZoneType != ZoneType.OnBattlefield)
            {
                // leave battlefield
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "remove", m_attackMod }),
                            new Commands.SendBehaviorMessage(card.Warrior, "DefenseModifiers", new object[] { "remove", m_defenseMod }));
                    }
                }
            }
        }

        public void RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                // enter battlefield
                if (command.Subject != Host
                    && command.Subject.Owner == Host.Owner
                    && Host.Warrior != null
                    && Host.IsOnBattlefield)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(Host.Warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                        new Commands.SendBehaviorMessage(Host.Warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
                }
            }
            else if (command.FromZoneType == ZoneType.OnBattlefield
                     && command.ToZoneType != ZoneType.OnBattlefield)
            {
                // leave battlefield
                if (command.Subject != Host
                    && command.Subject.Owner == Host.Owner
                    && Host.Warrior != null
                    && Host.IsOnBattlefield)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(Host.Warrior, "AttackModifiers", new object[] { "remove", m_attackMod }),
                        new Commands.SendBehaviorMessage(Host.Warrior, "DefenseModifiers", new object[] { "remove", m_defenseMod }));
                }
            }
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, Model.AttackBoost);
            m_defenseMod = new ValueModifier(ValueModifierOperator.Add, Model.DefenseBoost);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Passive_AllFriendWarriorAttackUpWhileDefenseUp).m_attackMod;
            m_defenseMod = (original as Passive_AllFriendWarriorAttackUpWhileDefenseUp).m_defenseMod;
        }

        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpWhileDefenseUp), DefaultName = "御柱特攻")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int DefenseBoost { get; set; }
        }
    }
}
