using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class ManaCost_PrePlay : BaseBehavior<ManaCost_PrePlay.ModelType>,
        Commands.IPrerequisiteTrigger<Commands.PlayCard>,
        Commands.IPrologTrigger<Commands.PlayCard>,
        IPlayable
	{
        Commands.Result Commands.IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.CommandContext context)
        {
            var command = context.Command as Commands.PlayCard;
            if (command.CardToPlay == Host)
            {
                if (!IsPlayable(context.Game))
                {
                    return Commands.Result.Cancel("Insufficient mana.");
                }

                context.Game.ReserveMana(Host.Owner, Model.Cost);
            }

            return Commands.Result.Pass;
        }

        void Commands.IPrologTrigger<Commands.PlayCard>.Run(Commands.CommandContext context)
        {
            var command = context.Command as Commands.PlayCard;
            if (command.CardToPlay == Host)
            {
                context.Game.IssueCommand(new Commands.UpdateMana { Player = Host.Owner, Amount = -Model.Cost, PreReserved = true });
            }
        }

		public bool IsPlayable(Game game)
		{
			return Host.Owner.FreeMana >= Model.Cost;
		}

		[BehaviorModel(typeof(ManaCost_PrePlay))]
		public class ModelType : BehaviorModel
		{
            public int Cost { get; set; }
		}
	}
}
