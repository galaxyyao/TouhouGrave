﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_AttackGrowth:
        BaseBehavior<Spell_AttackGrowth.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        private ValueModifier m_attackMod;

        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                "指定1张己方的卡，增加3点攻击力");
            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(
                Game.GetTargets(this)[0].Behaviors.Get<Warrior>(),
                "AttackModifiers",
                new object[] { "add", m_attackMod }));
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, 3);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Spell_AttackGrowth).m_attackMod;
        }

        [BehaviorModel(typeof(Spell_AttackGrowth), Category = "v0.5/Spell", DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
