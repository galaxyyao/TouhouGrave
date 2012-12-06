using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class ManaCost_PrePlay : BaseBehavior<ManaCost_PrePlay.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>,
        IPlayable
	{
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                if (!IsPlayable(context.Game))
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }

                context.Game.ReserveMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        void IPrologTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                context.Game.IssueCommands(new Commands.UpdateMana
                {
                    Player = Host.Owner,
                    Amount = -Model.Cost
                });
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
