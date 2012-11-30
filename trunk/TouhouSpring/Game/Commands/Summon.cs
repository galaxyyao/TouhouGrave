using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Directly play a card onto the 
    /// </summary>
    public class Summon : ICommand
    {
        public string Token
        {
            get { return "Summon"; }
        }

        // TODO: change to serializable behavior ID
        public ICardModel Model
        {
            get; set;
        }

        // TODO: change to serializable behavior ID
        public Player Owner
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Model == null)
            {
                throw new CommandValidationFailException("Card model can't be null.");
            }
            else if (Owner == null)
            {
                throw new CommandValidationFailException("Owner can't be null.");
            }
            else if (!game.Players.Contains(Owner))
            {
                throw new CommandValidationFailException("Owner player is not registered in game.");
            }
        }

        public void RunMain(Game game)
        {
            BaseCard summoned = new BaseCard(Model, Owner);
            Owner.m_battlefieldCards.Add(summoned);
        }
    }
}
