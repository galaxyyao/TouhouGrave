using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_HealAllAlly_NLife:
        BaseBehavior<Spell_HealAllAlly_NLife.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            foreach (var card in Game.ActingPlayer.CardsOnBattlefield)
            {
                Game.IssueCommands(new Commands.HealCard(card, Model.LifeToHeal, this));
            }
        }

        [BehaviorModel(typeof(Spell_HealAllAlly_NLife), DefaultName = "全体治愈")]
        public class ModelType : BehaviorModel
        {
            public int LifeToHeal
            {
                get;
                set;
            }
        }
    }
}
