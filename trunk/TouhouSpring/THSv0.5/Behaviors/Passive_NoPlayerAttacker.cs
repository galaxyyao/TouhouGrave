using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_NoPlayerAttacker:
        BaseBehavior<Passive_NoPlayerAttacker.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DrawCard>,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        bool isAttackedThisTurn = false;

        public void RunEpilog(Commands.DrawCard command)
        {
            if (Game.ActingPlayer != Host.Owner)
                return;
            isAttackedThisTurn = false;
            //TODO: Future change for 3 or more players
            if(Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count==0)
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
        }

        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (Game.ActingPlayer == Host.Owner
                && Host.IsOnBattlefield
                && command.Cause == Host.Behaviors.Get<Warrior>())
                isAttackedThisTurn = true;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (Game.ActingPlayer != Host.Owner
                && command.CardToPlay.Owner == Host.Owner
                && !Host.IsOnBattlefield)
                return;
            //TODO: Future change for 3 or more players
            if (Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count > 0
                && !isAttackedThisTurn)
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
        }

        [BehaviorModel(typeof(Passive_NoPlayerAttacker), DefaultName = "不可攻击玩家")]
        public class ModelType : BehaviorModel
        { }
    }
}
