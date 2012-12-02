using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFriendWarriorAttackUpAndDefenseUp :
        BaseBehavior<Passive_AllFriendWarriorAttackUpAndDefenseUp.ModelType>,
        IEpilogTrigger<PlayCard>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, 1);
        private readonly Warrior.ValueModifier m_defenseMod = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, 1);

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;

                    context.Game.IssueCommands(
                        new SendBehaviorMessage
                        {
                            Target = card.Behaviors.Get<Warrior>(),
                            Message = "AttackModifiers",
                            Args = new object[] { "add", m_attackMod }
                        },
                        new SendBehaviorMessage
                        {
                            Target = card.Behaviors.Get<Warrior>(),
                            Message = "DefenseModifiers",
                            Args = new object[] { "add", m_defenseMod }
                        });
                }
            }
            else if (context.Command.CardToPlay.Owner == Host.Owner
                     && IsOnBattlefield
                     && context.Command.CardToPlay.Behaviors.Get<Warrior>() != null)
            {
                context.Game.IssueCommands(
                    new SendBehaviorMessage
                    {
                        Target = context.Command.CardToPlay.Behaviors.Get<Warrior>(),
                        Message = "AttackModifiers",
                        Args = new object[] { "add", m_attackMod }
                    },
                    new SendBehaviorMessage
                    {
                        Target = context.Command.CardToPlay.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "add", m_defenseMod }
                    });
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                    {
                        context.Game.IssueCommands(
                            new SendBehaviorMessage
                            {
                                Target = card.Behaviors.Get<Warrior>(),
                                Message = "AttackModifiers",
                                Args = new object[] { "remove", m_attackMod }
                            },
                            new SendBehaviorMessage
                            {
                                Target = card.Behaviors.Get<Warrior>(),
                                Message = "DefenseModifiers",
                                Args = new object[] { "remove", m_defenseMod }
                            });
                    }
                }
            }
            else if (context.CardToLeft.Owner == Host.Owner
                     && IsOnBattlefield)
            {
                context.Game.IssueCommands(
                    new SendBehaviorMessage
                    {
                        Target = context.CardToLeft.Behaviors.Get<Warrior>(),
                        Message = "AttackModifiers",
                        Args = new object[] { "remove", m_attackMod }
                    },
                    new SendBehaviorMessage
                    {
                        Target = context.CardToLeft.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "remove", m_defenseMod }
                    });
            }
        }

        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpAndDefenseUp), DefaultName = "秘药")]
        public class ModelType : BehaviorModel
        { }
    }
}
