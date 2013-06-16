using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AllFriendWarriorAttackUpWhileDefenseDown :
        BaseBehavior<Passive_AllFriendWarriorAttackUpWhileDefenseDown.ModelType>,
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>,
        IEpilogTrigger<Commands.KillMove<Commands.Battlefield>>
    {
        private ValueModifier m_attackMod;
        private ValueModifier m_defenseMod;

        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                        new Commands.SendBehaviorMessage(warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
                }
            }
            else if (command.Subject.Owner == Host.Owner
                     && Host.IsOnBattlefield
                     && command.Subject.Behaviors.Get<Warrior>() != null)
            {
                var warrior = command.Subject.Behaviors.Get<Warrior>();
                Game.QueueCommands(
                    new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                    new Commands.SendBehaviorMessage(warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
            }
        }

        public void RunEpilog(Commands.KillMove<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(
                                card.Behaviors.Get<Warrior>(),
                                "AttackModifiers",
                                new object[] { "remove", m_attackMod }),
                            new Commands.SendBehaviorMessage(
                                card.Behaviors.Get<Warrior>(),
                                "DefenseModifiers",
                                new object[] { "remove", m_defenseMod }));
                    }
                }
            }
            else if (command.Subject.Owner == Host.Owner
                     && Host.IsOnBattlefield)
            {
                Game.QueueCommands(
                    new Commands.SendBehaviorMessage(
                        command.Subject.Behaviors.Get<Warrior>(),
                        "AttackModifiers",
                        new object[] { "remove", m_attackMod }),
                    new Commands.SendBehaviorMessage(
                        command.Subject.Behaviors.Get<Warrior>(),
                        "DefenseModifiers",
                        new object[] { "remove", m_defenseMod }));
            }
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, 2);
            m_defenseMod = new ValueModifier(ValueModifierOperator.Add, -1);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Passive_AllFriendWarriorAttackUpWhileDefenseDown).m_attackMod;
            m_defenseMod = (original as Passive_AllFriendWarriorAttackUpWhileDefenseDown).m_defenseMod;
        }

        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpWhileDefenseDown), DefaultName = "御柱特攻")]
        public class ModelType : BehaviorModel
        { }
    }
}
