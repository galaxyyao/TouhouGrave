using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeterAura_NAttack :
        BaseBehavior<Passive_DeterAura_NAttack.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.Summon>,
        IEpilogTrigger<Commands.Kill>
    {
        private ValueModifier m_attackMod;

        public void RunEpilog(Commands.PlayCard command)
        {
            //TODO: Future change for 3 or more players
            if (!Host.IsOnBattlefield)
                return;
            if (command.CardToPlay == Host)
            {
                foreach (var card in Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield))
                {
                    AffectByAura(card);
                }
            }
            else if (command.CardToPlay.Owner != Host.Owner)
            {
                AffectByAura(command.CardToPlay);
            }
        }

        public void RunEpilog(Commands.Summon command)
        {
            if (command.CardSummoned.Owner != Host.Owner)
            {
                AffectByAura(command.CardSummoned);
            }
        }

        private void AffectByAura(CardInstance card)
        {
            var warrior = card.Behaviors.Get<Warrior>();
            if (warrior != null)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
            }
        }

        private void LeaveAura(CardInstance card)
        {
            var warrior = card.Behaviors.Get<Warrior>();
            if (warrior != null)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
            }
        }

        public void RunEpilog(Commands.Kill command)
        {
            if (command.Target == Host)
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
