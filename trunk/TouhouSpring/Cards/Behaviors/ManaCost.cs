using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaCost : BaseBehavior<ManaCost.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>,
        IPrerequisiteTrigger<Commands.ActivateAssist>,
        IEpilogTrigger<Commands.ActivateAssist>,
        IPrerequisiteTrigger<Commands.Redeem>,
        IEpilogTrigger<Commands.Redeem>
    {
        public int Cost
        {
            get { return Model.Cost; }
        }

        public CommandResult RunPrerequisite(Commands.PlayCard command)
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

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -Model.Cost, this));
            }
        }

        public CommandResult RunPrerequisite(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                if (Host.Owner.FreeMana < Model.Cost)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }

                Game.ReserveMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -Model.Cost, this));
            }
        }

        public CommandResult RunPrerequisite(Commands.Redeem command)
        {
            if (command.Target == Host)
            {
                if (Host.Owner.FreeMana < Model.Cost)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }

                Game.ReserveMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.Redeem command)
        {
            if (command.Target == Host)
            {
                Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -Model.Cost, this));
            }
        }

        [BehaviorModel(typeof(ManaCost))]
        public class ModelType : BehaviorModel
        {
            public int Cost { get; set; }
        }
    }
}
