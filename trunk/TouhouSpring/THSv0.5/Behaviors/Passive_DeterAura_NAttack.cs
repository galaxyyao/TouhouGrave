using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeterAura_NAttack :
        BaseBehavior<Passive_DeterAura_NAttack.ModelType>,
        Commands.ICause,
        // TODO: Commands.MoveTo<Commands.Battlefield>
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>,
        IEpilogTrigger<Commands.SummonMove<Commands.Battlefield>>,
        // TODO: Commands.Kill
        IEpilogTrigger<Commands.KillMove<Commands.Battlefield>>
    {
        private ValueModifier m_attackMod;

        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            //TODO: Future change for 3 or more players
            if (!Host.IsOnBattlefield)
                return;
            if (command.Subject == Host)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield))
                {
                    AffectByAura(card);
                }
            }
            else if (command.Subject.Owner != Host.Owner)
            {
                AffectByAura(command.Subject);
            }
        }

        public void RunEpilog(Commands.SummonMove<Commands.Battlefield> command)
        {
            if (command.Subject.Owner != Host.Owner)
            {
                AffectByAura(command.Subject);
            }
        }

        private void AffectByAura(CardInstance card)
        {
            var warrior = card.Behaviors.Get<Warrior>();
            if (warrior != null)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        private void LeaveAura(CardInstance card)
        {
            var warrior = card.Behaviors.Get<Warrior>();
            if (warrior != null)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
            }
        }

        public void RunEpilog(Commands.KillMove<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield))
                {
                    LeaveAura(card);
                }
            }
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
