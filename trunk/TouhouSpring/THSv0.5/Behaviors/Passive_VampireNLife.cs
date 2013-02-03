using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_VampireNLife :
        BaseBehavior<Passive_VampireNLife.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if(command.Cause==Host.Behaviors.Get<Warrior>()
                && Host.IsOnBattlefield
                && Game.ActingPlayer==Host.Owner)
            {
                Game.IssueCommands(new Commands.HealCard(Host, Model.LifeToHeal, this));
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "吸血")]
        public class ModelType : BehaviorModel<Passive_VampireNLife>
        {
            public int LifeToHeal
            {
                get;
                set;
            }
        }
    }
}
