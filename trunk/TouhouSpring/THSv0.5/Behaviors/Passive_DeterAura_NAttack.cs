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
                foreach (var card in Game.ActingPlayerEnemies.First().CardsOnBattlefield)
                {
                    AffectedByAura(card);
                }
            }
            if (command.CardToPlay.Owner != Host.Owner)
            {
                AffectedByAura(command.CardToPlay);
            }
        }

        private void AffectedByAura(BaseCard card)
        {
            if (!card.Behaviors.Has<Warrior>())
                return;
            var warrior = card.Behaviors.Get<Warrior>();
            Game.IssueCommands(new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
        }

        private void LeaveAura(BaseCard card)
        {
            if (!card.Behaviors.Has<Warrior>())
                return;
            Game.IssueCommands(
                            new Commands.SendBehaviorMessage(
                                card.Behaviors.Get<Warrior>(),
                                "AttackModifiers",
                                new object[] { "remove", m_attackMod }
                                ));
        }

        public void RunEpilog(Commands.Kill command)
        {
            if (command.Target != Host)
                return;
            if (Game.ActingPlayer == Host.Owner)
            {
                foreach (var card in Game.ActingPlayerEnemies.First().CardsOnBattlefield)
                {
                    LeaveAura(card);
                }
            }
            else
            {
                foreach (var card in Game.ActingPlayer.CardsOnBattlefield)
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

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "灵压")]
        public class ModelType : BehaviorModel<Passive_DeterAura_NAttack>
        {
        }
    }
}
