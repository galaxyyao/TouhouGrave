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
        IEpilogTrigger<Commands.DeactivateAssist>,
        IEpilogTrigger<Commands.IMoveCard>
    {
        private ValueModifier m_attackMod;

        public void RunEpilog(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    var warrior = card.Behaviors.Get<Warrior>();
                    if (warrior == null)
                        continue;

                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
            }
        }

        public void RunEpilog(Commands.DeactivateAssist command)
        {
            if (command.CardToDeactivate == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    var warrior = card.Behaviors.Get<Warrior>();
                    if (warrior == null)
                        continue;

                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                }
            }
        }

        public void RunEpilog(Commands.IMoveCard command)
        {
            if (Host.IsActivatedAssist
                && Host.Owner == command.Subject.Owner
                && Host != command.Subject)
            {
                if (command.FromZoneType != ZoneType.OnBattlefield
                    && command.ToZoneType == ZoneType.OnBattlefield)
                {
                    var warrior = command.Subject.Behaviors.Get<Warrior>();
                    if (warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                    }
                }
                else if (command.FromZoneType == ZoneType.OnBattlefield
                         && command.ToZoneType != ZoneType.OnBattlefield)
                {
                    var warrior = command.Subject.Behaviors.Get<Warrior>();
                    if (warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                    }
                }
            }
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, 1);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Passive_Aura_Offense_NAttack).m_attackMod;
        }

        [BehaviorModel(typeof(Passive_Aura_Offense_NAttack), Category = "v0.5/Passive", DefaultName = "进攻光环")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
