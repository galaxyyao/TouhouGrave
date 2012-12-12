using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AllFriendWarriorAttackUpAndDefenseUp :
        BaseBehavior<Passive_AllFriendWarriorAttackUpAndDefenseUp.ModelType>,
        IEpilogTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.Kill>
    {
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 1);
        private readonly Warrior.ValueModifier m_defenseMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 1);

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                        new Commands.SendBehaviorMessage(warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
                }
            }
            else if (command.CardToPlay.Owner == Host.Owner
                     && IsOnBattlefield
                     && command.CardToPlay.Behaviors.Get<Warrior>() != null)
            {
                var warrior = command.CardToPlay.Behaviors.Get<Warrior>();
                Game.IssueCommands(
                    new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }),
                    new Commands.SendBehaviorMessage(warrior, "DefenseModifiers", new object[] { "add", m_defenseMod }));
            }
        }

        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
        {
            if (!command.LeftBattlefield)
            {
                return;
            }

            if (command.Target == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                    {
                        Game.IssueCommands(
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
            else if (command.Target.Owner == Host.Owner
                     && IsOnBattlefield)
            {
                Game.IssueCommands(
                    new Commands.SendBehaviorMessage(
                        command.Target.Behaviors.Get<Warrior>(),
                        "AttackModifiers",
                        new object[] { "remove", m_attackMod }),
                    new Commands.SendBehaviorMessage(
                        command.Target.Behaviors.Get<Warrior>(),
                        "DefenseModifiers",
                        new object[] { "remove", m_defenseMod }));
            }
        }

        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpAndDefenseUp), DefaultName = "秘药")]
        public class ModelType : BehaviorModel
        { }
    }
}
