using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorAttackFirst :
        BaseBehavior<Passive_WarriorAttackFirst.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private Warrior.ValueModifier attackFirstCompensation = null;

        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Cause == Host.Behaviors.Get<Warrior>())
            {
                // TODO: looks like this impl won't work...
                var warriorAttackedBhv = command.Target.Behaviors.Get<Warrior>();
                if (warriorAttackedBhv.AccumulatedDamage >= warriorAttackedBhv.Defense)
                {
                    int damageWontDeal = command.Target.Behaviors.Get<Warrior>().Attack;
                    attackFirstCompensation = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, damageWontDeal);
                    Game.IssueCommands(new Commands.SendBehaviorMessage(
                        Host.Behaviors.Get<Warrior>(),
                        "DefenseModifiers",
                        new object[] { "add", attackFirstCompensation }));
                }
            }
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(Commands.EndTurn command)
        {
            if (attackFirstCompensation != null)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Host.Behaviors.Get<Warrior>(),
                    "DefenseModifiers",
                    new object[] { "remove", attackFirstCompensation }));
                attackFirstCompensation = null;
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackFirst), DefaultName = "风神")]
        public class ModelType : BehaviorModel
        { }
    }
}
