using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HeroAttackUpWithCardNumber
        : BaseBehavior<Passive_HeroAttackUpWithCardNumber.ModelType>,
        IEpilogTrigger<Commands.Kill>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private Warrior.ValueModifier m_attackModifier = null;

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                || Host.IsOnBattlefield && command.CardToPlay.Owner == Host.Owner)
            {

                UpdateNumber();
            }
        }

        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
        {
            if (command.LeftBattlefield
                && (command.Target == Host
                    || Host.IsOnBattlefield && command.Target.Owner == Host.Owner))
            {
                UpdateNumber();
            }
        }

        private void UpdateNumber()
        {
            if (!Host.Behaviors.Has<Warrior>() || !Host.Behaviors.Has<Hero>())
            {
                return;
            }

            int numberOfWarriors = Host.IsOnBattlefield
                                   ? Host.Owner.CardsOnBattlefield.Count(
                                        card => card.Behaviors.Has<Warrior>() && !card.Behaviors.Has<Hero>())
                                   : 0;

            if (m_attackModifier != null && m_attackModifier.Amount != numberOfWarriors)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Host.Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "remove", m_attackModifier }));
                m_attackModifier = null;
            }
            if (m_attackModifier == null)
            {
                m_attackModifier = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, numberOfWarriors);
                Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Host.Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "add", m_attackModifier }));
            }
        }

        [BehaviorModel(typeof(Passive_HeroAttackUpWithCardNumber), DefaultName = "未来永劫斩")]
        public class ModelType : BehaviorModel
        { }
    }
}
