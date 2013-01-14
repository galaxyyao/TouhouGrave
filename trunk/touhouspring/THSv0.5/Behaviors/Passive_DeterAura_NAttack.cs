﻿using System;
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
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, -1);

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
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

        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
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

        [BehaviorModel(typeof(Passive_DeterAura_NAttack), DefaultName = "灵压")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
