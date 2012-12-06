using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFriendWarriorAttackUpWhileDefenseDown :
        BaseBehavior<Passive_AllFriendWarriorAttackUpWhileDefenseDown.ModelType>,
        IEpilogTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.Kill>
    {
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, 2);
        private readonly Warrior.ValueModifier m_defenseMod = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, -1);

        void IEpilogTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
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
                        new Commands.SendBehaviorMessage
                        {
                            Target = card.Behaviors.Get<Warrior>(),
                            Message = "AttackModifiers",
                            Args = new object[] { "add", m_attackMod }
                        },
                        new Commands.SendBehaviorMessage
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
                    new Commands.SendBehaviorMessage
                    {
                        Target = context.Command.CardToPlay.Behaviors.Get<Warrior>(),
                        Message = "AttackModifiers",
                        Args = new object[] { "add", m_attackMod }
                    },
                    new Commands.SendBehaviorMessage
                    {
                        Target = context.Command.CardToPlay.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "add", m_defenseMod }
                    });
            }
        }

        void IEpilogTrigger<Commands.Kill>.Run(CommandContext<Commands.Kill> context)
        {
            if (!context.Command.LeftBattlefield)
            {
                return;
            }

            if (context.Command.Target == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                    {
                        context.Game.IssueCommands(
                            new Commands.SendBehaviorMessage
                            {
                                Target = card.Behaviors.Get<Warrior>(),
                                Message = "AttackModifiers",
                                Args = new object[] { "remove", m_attackMod }
                            },
                            new Commands.SendBehaviorMessage
                            {
                                Target = card.Behaviors.Get<Warrior>(),
                                Message = "DefenseModifiers",
                                Args = new object[] { "remove", m_defenseMod }
                            });
                    }
                }
            }
            else if (context.Command.Target.Owner == Host.Owner
                     && IsOnBattlefield)
            {
                context.Game.IssueCommands(
                    new Commands.SendBehaviorMessage
                    {
                        Target = context.Command.Target.Behaviors.Get<Warrior>(),
                        Message = "AttackModifiers",
                        Args = new object[] { "remove", m_attackMod }
                    },
                    new Commands.SendBehaviorMessage
                    {
                        Target = context.Command.Target.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "remove", m_defenseMod }
                    });
            }
        }

        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpWhileDefenseDown), DefaultName = "御柱特攻")]
        public class ModelType : BehaviorModel
        { }
    }
}
