using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class RemoveBehavior : ICommand
    {
        public string Token
        {
            get { return "RemoveBehavior"; }
        }

        // TODO: change to serialization-friendly ID
        public BaseCard Target
        {
            get; set;
        }

        // TODO: change to serialization-friendly ID
        public Behaviors.IBehavior Behavior
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Target == null)
            {
                throw new CommandValidationFailException("Target card can't be null.");
            }
            else if (!game.Players.Contains(Target.Owner))
            {
                throw new CommandValidationFailException("Target's owner is not registered in game.");
            }
            else if (Behavior == null)
            {
                throw new CommandValidationFailException("Behavior to be added can't be null.");
            }
            else if (!Target.Behaviors.Contains(Behavior))
            {
                throw new CommandValidationFailException("Behavior is not bound to the target card.");
            }
        }

        public void RunMain(Game game)
        {
            Target.Behaviors.Remove(Behavior);
        }
    }
}
