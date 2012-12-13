using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaCost_PrePlay : BaseBehavior<ManaCost_PrePlay.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>
	{
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                if (Host.Owner.FreeMana < Model.Cost)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }

                Game.ReserveMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        void IPrologTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -Model.Cost, this));
            }
        }

		[BehaviorModel(typeof(ManaCost_PrePlay))]
		public class ModelType : BehaviorModel
		{
            public int Cost { get; set; }
		}
	}
}
