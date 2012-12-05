using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DealDamageToCard : ICommand
    {
        public string Token
        {
            get { return "DealDamageToCard"; }
        }

        // TODO: change to some serializable reference
        public BaseCard Target
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
            else if (!game.Players.Contains(Target.Owner))
            {
                throw new CommandValidationFailException("The Player object is not registered in game.");
            }
            else if (!Target.Owner.CardsOnBattlefield.Contains(Target))
            {
                throw new CommandValidationFailException("Damage can only be dealt to cards on battlefield.");
            }
            else if (!Target.Behaviors.Has<Behaviors.Warrior>())
            {
                throw new CommandValidationFailException("Damage can only be dealt to non-warrior cards.");
            }
            else if (DamageToDeal <= 0)
            {
                throw new CommandValidationFailException("Damage must be greater than zero.");
            }
        }

        public void RunMain(Game game)
        {
            Debug.Assert(Target != null);
            Debug.Assert(Target.Behaviors.Has<Behaviors.Warrior>());
            Debug.Assert(DamageToDeal >= 0);
            Target.Behaviors.Get<Behaviors.Warrior>().AccumulatedDamage += DamageToDeal;
        }
    }
}
