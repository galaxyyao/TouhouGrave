using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DealDamageToPlayer : ICommand
    {
        public string Token
        {
            get { return "DealDamageToPlayer"; }
        }

        public Player Target
        {
            get; set;
        }

        // TODO: change to some serializable reference
        public Behaviors.IBehavior Cause
        {
            get; set;
        }

        public int DamageToDeal
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Target == null)
            {
                throw new CommandValidationFailException("Target player can't be null.");
            }
            else if (!game.Players.Contains(Target))
            {
                throw new CommandValidationFailException("The Player object is not registered in game.");
            }
            else if (DamageToDeal <= 0)
            {
                throw new CommandValidationFailException("Damage must be greater than zero.");
            }
        }

        public void RunMain(Game game)
        {
            Target.Health -= Math.Min(DamageToDeal, 0);
        }
    }
}
