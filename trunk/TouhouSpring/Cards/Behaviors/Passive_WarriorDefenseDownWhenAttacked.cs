using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDefenseDownWhenAttacked
        : BaseBehavior<Passive_WarriorDefenseDownWhenAttacked.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        Commands.IEpilogTrigger<Commands.EndTurn>
    {
        private bool isBlockedLastRound = false;

        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.CardDamaged == Host
                && context.Game.PlayerPlayer != Host.Owner)
                isBlockedLastRound = true;
        }

        void Commands.IEpilogTrigger<Commands.EndTurn>.Run(Commands.CommandContext context)
        {
            if (context.Game.PlayerPlayer != Host.Owner && isBlockedLastRound)
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //isBlockedLastRound = false;
                //Func<int, int> defenseMod = y => y - 1;
                //Host.Behaviors.Get<Warrior>().Defense.AddModifierToTail(defenseMod);
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDefenseDownWhenAttacked), DefaultName = "日光折射")]
        public class ModelType : BehaviorModel
        { }
    }
}
