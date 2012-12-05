﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackFirst :
        BaseBehavior<Passive_WarriorAttackFirst.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private Warrior.ValueModifier attackFirstCompensation = null;

        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.Cause == Host.Behaviors.Get<Warrior>())
            {
                var warriorAttackedBhv=context.CardDamaged.Behaviors.Get<Warrior>();
                if (warriorAttackedBhv.AccumulatedDamage >= warriorAttackedBhv.Defense)
                {
                    int damageWontDeal = context.CardDamaged.Behaviors.Get<Warrior>().Attack;
                    attackFirstCompensation = new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, damageWontDeal);
                    context.Game.IssueCommands(new Commands.SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "add", attackFirstCompensation }
                    });
                }
            }
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(CommandContext<Commands.EndTurn> context)
        {
            if (attackFirstCompensation != null)
            {
                context.Game.IssueCommands(new Commands.SendBehaviorMessage
                {
                    Target = Host.Behaviors.Get<Warrior>(),
                    Message = "DefenseModifiers",
                    Args = new object[] { "remove", attackFirstCompensation }
                });
                attackFirstCompensation = null;
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackFirst), DefaultName = "风神")]
        public class ModelType : BehaviorModel
        { }
    }
}
