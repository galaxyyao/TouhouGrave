﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Aura_Offense_NAttack:
        BaseBehavior<Passive_Aura_Offense_NAttack.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.ActivateAssist>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 1);

        public void RunEpilog(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
            }
            if (command.PreviouslyActivatedCard == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                }
            }
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (Host.Owner.ActivatedAssist == Host
                && Host.Owner == command.CardToPlay.Owner)
            {
                var warrior = command.CardToPlay.Behaviors.Get<Warrior>();
                if (warrior != null)
                {
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
            }
        }

        [BehaviorModel(typeof(Passive_Aura_Offense_NAttack), DefaultName = "进攻光环")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
