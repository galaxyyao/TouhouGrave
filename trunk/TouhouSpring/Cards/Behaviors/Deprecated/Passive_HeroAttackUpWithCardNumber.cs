using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HeroAttackUpWithCardNumber
        : BaseBehavior<Passive_HeroAttackUpWithCardNumber.ModelType>,
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>,
        IEpilogTrigger<Commands.KillMove<Commands.Battlefield>>
    {
        private ValueModifier m_attackModifier;

        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host
                || Host.IsOnBattlefield && command.Subject.Owner == Host.Owner)
            {
                UpdateNumber();
            }
        }

        public void RunEpilog(Commands.KillMove<Commands.Battlefield> command)
        {
            if (command.Subject == Host
                || Host.IsOnBattlefield && command.Subject.Owner == Host.Owner)
            {
                UpdateNumber();
            }
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackModifier = (original as Passive_HeroAttackUpWithCardNumber).m_attackModifier;
        }

        private void UpdateNumber()
        {
            if (Host.Owner.Hero == null || !Host.Owner.Hero.Behaviors.Has<Warrior>())
            {
                return;
            }

            int numberOfWarriors = Host.IsOnBattlefield
                                   ? Host.Owner.CardsOnBattlefield.Count(
                                        card => card.Behaviors.Has<Warrior>() && !card.Behaviors.Has<Hero>())
                                   : 0;

            if (m_attackModifier != null && m_attackModifier.Amount != numberOfWarriors)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(
                    Host.Owner.Hero.Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "remove", m_attackModifier }));
                m_attackModifier = null;
            }
            if (m_attackModifier == null && numberOfWarriors != 0)
            {
                m_attackModifier = new ValueModifier(ValueModifierOperator.Add, numberOfWarriors);
                Game.QueueCommands(new Commands.SendBehaviorMessage(
                    Host.Owner.Hero.Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "add", m_attackModifier }));
            }
        }

        [BehaviorModel(typeof(Passive_HeroAttackUpWithCardNumber), DefaultName = "未来永劫斩")]
        public class ModelType : BehaviorModel
        { }
    }
}
