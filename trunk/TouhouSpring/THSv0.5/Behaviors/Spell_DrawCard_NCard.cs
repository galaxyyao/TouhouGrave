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
        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Model.CardToDraw.Repeat(() => Game.IssueCommands(new Commands.DrawCard(Host.Owner)));
            }
        }

        [BehaviorModel(Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel<Spell_DrawCard_NCard>
        {
            public int CardToDraw
            {
                get;
                set;
            }
        }
    }
}
