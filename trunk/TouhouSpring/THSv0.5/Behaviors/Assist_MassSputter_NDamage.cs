using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_MassSputter_NDamage :
        BaseBehavior<Assist_MassSputter_NDamage.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.DealDamageToPlayer>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (Host.Owner.ActivatedAssist == Host
                && Game.ActingPlayer==Host.Owner
                && command.Cause is Warrior
                && ((Warrior)command.Cause).Host.Owner == Host.Owner)
            {
                Game.IssueCommands(new Commands.DealDamageToPlayer(Game.ActingPlayerEnemies.First(), Model.DamageToDeal, this));
            }
        }

        void IEpilogTrigger<Commands.DealDamageToPlayer>.Run(Commands.DealDamageToPlayer command)
        {
            if (Host.Owner.ActivatedAssist == Host
                && Game.ActingPlayer == Host.Owner
                && command.Cause is Warrior
                && ((Warrior)command.Cause).Host.Owner == Host.Owner)
            {
                Game.IssueCommands(new Commands.DealDamageToPlayer(Game.ActingPlayerEnemies.First(), Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Assist_MassSputter_NDamage), DefaultName = "大规模溅射")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get;
                set;
            }
        }
    }
}
