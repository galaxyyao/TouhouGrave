using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class AddBehavior : ICommand
    {
        public string Token
        {
            get { return "AddBehavior"; }
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
            else if (Behavior.Host != null)
            {
                throw new CommandValidationFailException("Behavior can't be bound already.");
            }
            else if (Behavior.Persistent)
            {
                throw new CommandValidationFailException("Persistent behavior can't be added dynamically.");
            }
        }

        public void RunMain(Game game)
        {
            Target.Behaviors.Add(Behavior);
        }
    }
}
