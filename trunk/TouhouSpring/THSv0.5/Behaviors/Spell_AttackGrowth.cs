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
        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Warrior != null),
                "指定1张己方的卡，增加3点攻击力");
            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(
                Game.GetTargets(this)[0].Warrior,
                WarriorMessage.AddAttackModifier,
                Model.Modifier));
        }

        [BehaviorModel(typeof(Spell_AttackGrowth), Category = "v0.5/Spell", DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
            public int Amount
            {
                get { return Modifier.Amount; }
                set { Modifier = new ValueModifier(ValueModifierOperator.Add, value, false); }
            }

            [System.ComponentModel.Browsable(false)]
            public ValueModifier Modifier
            {
                get; private set;
            }

            public ModelType()
            {
                Modifier = new ValueModifier(ValueModifierOperator.Add, 1);
            }
        }
    }
}
