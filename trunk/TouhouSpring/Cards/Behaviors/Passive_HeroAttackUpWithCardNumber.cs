using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_HeroAttackUpWithCardNumber
        : BaseBehavior<Passive_HeroAttackUpWithCardNumber.ModelType>,
        IEpilogTrigger<Kill>,
        IEpilogTrigger<PlayCard>
    {
        private Warrior.ValueModifier m_attackModifier = null;

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host
                || IsOnBattlefield && context.Command.CardToPlay.Owner == Host.Owner)
            {
                
                UpdateNumber(context.Game);
            }
        }

        void IEpilogTrigger<Kill>.Run(CommandContext<Kill> context)
        {
            if (context.Command.LeftBattlefield
                && (context.Command.Target == Host
                    || IsOnBattlefield && context.Command.Target.Owner == Host.Owner))
            {
                UpdateNumber(context.Game);
            }
        }

        private void UpdateNumber(Game game)
        {
            if (!Host.Behaviors.Has<Warrior>() || !Host.Behaviors.Has<Hero>())
            {
                return;
            }

            int numberOfWarriors = !IsOnBattlefield
                ? 0
                : Host.Owner.CardsOnBattlefield.Count(
                    card => card.Behaviors.Has<Warrior>() && !card.Behaviors.Has<Hero>());

            if (m_attackModifier != null && m_attackModifier.Amount != numberOfWarriors)
            {
                game.IssueCommands(new SendBehaviorMessage
                {
                    Target = Host.Behaviors.Get<Warrior>(),
                    Message = "AttackModifiers",
                    Args = new object[] { "remove", m_attackModifier }
                });
                m_attackModifier = null;
            }
            if (m_attackModifier == null)
            {
                m_attackModifier = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, numberOfWarriors);
                game.IssueCommands(new SendBehaviorMessage
                {
                    Target = Host.Behaviors.Get<Warrior>(),
                    Message = "AttackModifiers",
                    Args = new object[] { "add", m_attackModifier }
                });
            }
        }

        [BehaviorModel(typeof(Passive_HeroAttackUpWithCardNumber), DefaultName = "未来永劫斩")]
        public class ModelType : BehaviorModel
        { }
    }
}
