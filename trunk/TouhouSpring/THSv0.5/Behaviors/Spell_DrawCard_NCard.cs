using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DrawCard_NCard:
        BaseBehavior<Spell_DrawCard_NCard.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Model.CardToDraw.Repeat(() => Game.IssueCommands(new Commands.DrawCard(Host.Owner)));
            }
        }

        [BehaviorModel(typeof(Spell_DrawCard_NCard))]
        public class ModelType : BehaviorModel
        {
            public int CardToDraw
            {
                get;
                set;
            }
        }
    }
}
