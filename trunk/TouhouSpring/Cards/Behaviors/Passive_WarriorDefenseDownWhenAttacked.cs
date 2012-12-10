using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDefenseDownWhenAttacked
        : BaseBehavior<Passive_WarriorDefenseDownWhenAttacked.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private bool isBlockedLastRound = false;

        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Target == Host
                && command.Game.PlayerPlayer != Host.Owner)
                isBlockedLastRound = true;
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(Commands.EndTurn command)
        {
            if (command.Game.PlayerPlayer != Host.Owner
                && Host.Behaviors.Has<Warrior>()
                && isBlockedLastRound)
            {
                isBlockedLastRound = false;
                command.Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Host.Behaviors.Get<Warrior>(),
                    "DefenseModifiers",
                    new object[] { "add", new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, -1) }));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDefenseDownWhenAttacked), DefaultName = "日光折射")]
        public class ModelType : BehaviorModel
        { }
    }
}
