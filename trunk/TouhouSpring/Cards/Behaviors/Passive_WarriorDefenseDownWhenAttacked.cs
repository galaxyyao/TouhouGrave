using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDefenseDownWhenAttacked
        : BaseBehavior<Passive_WarriorDefenseDownWhenAttacked.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private bool isBlockedLastRound = false;

        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.CardDamaged == Host
                && context.Game.PlayerPlayer != Host.Owner)
                isBlockedLastRound = true;
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(CommandContext<Commands.EndTurn> context)
        {
            if (context.Game.PlayerPlayer != Host.Owner
                && Host.Behaviors.Has<Warrior>()
                && isBlockedLastRound)
            {
                isBlockedLastRound = false;
                context.Game.IssueCommands(new Commands.SendBehaviorMessage
                {
                    Target = Host.Behaviors.Get<Warrior>(),
                    Message = "DefenseModifiers",
                    Args = new object[] { "add", new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, -1) }
                });
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDefenseDownWhenAttacked), DefaultName = "日光折射")]
        public class ModelType : BehaviorModel
        { }
    }
}
