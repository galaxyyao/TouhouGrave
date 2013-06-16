using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DrawCard_NCard:
        BaseBehavior<Spell_DrawCard_NCard.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>
    {
        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                Model.CardToDraw.Repeat(() => Game.QueueCommands(new Commands.DrawMove<Commands.Hand>(Host.Owner)));
            }
        }

        [BehaviorModel(typeof(Spell_DrawCard_NCard), Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel
        {
            public int CardToDraw
            {
                get; set;
            }
        }
    }
}
