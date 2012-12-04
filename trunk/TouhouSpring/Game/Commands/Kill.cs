using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Kill : ICommand
    {
        public string Token
        {
            get { return "Kill"; }
        }

        // TODO: change to serialization-friendly ID
        public BaseCard Target
        {
            get; set;
        }

        // TODO: change to serialization-friendly ID
        public Behaviors.IBehavior Cause
        {
            get; set;
        }

        public bool LeftBattlefield
        {
            get; private set;
        }

        public bool EnteredGraveyard
        {
            get; private set;
        }

        public void Validate(Game game)
        {
            if (Target == null)
            {
                throw new CommandValidationFailException("Target card can't be null.");
            }
            else if (!game.Players.Contains(Target.Owner))
            {
                throw new CommandValidationFailException("Target card's owner player is not registered in the game.");
            }
            else if (!Target.Owner.CardsOnBattlefield.Contains(Target)
                     && !Target.Owner.CardsOnHand.Contains(Target))
            {
                throw new CommandValidationFailException("Target card is not on battlefield nor on hand.");
            }
        }

        public void RunMain(Game game)
        {
            Debug.Assert(Target != null);
            if (Target.Owner.CardsOnBattlefield.Contains(Target))
            {
                Target.Owner.m_battlefieldCards.Remove(Target);
                LeftBattlefield = true;
            }
            else if (Target.Owner.CardsOnHand.Contains(Target))
            {
                Target.Owner.m_handSet.Remove(Target);
            }
            else
            {
                Debug.Assert(false);
            }

            // reset card states
            for (int i = 0; i < Target.Behaviors.Count; ++i)
            {
                if (!Target.Behaviors[i].Persistent)
                {
                    Target.Behaviors.RemoveAt(i);
                    --i;
                }
                else
                {
                    // TODO: reset behavior states
                }
            }

            if (Target != Target.Owner.Hero.Host)
            {
                Target.Owner.m_graveyard.AddCardToTop(Target);
                EnteredGraveyard = true;
            }
        }
    }
}
