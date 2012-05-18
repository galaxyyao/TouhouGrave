using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    public class CardEnteredGraveyardContext : TriggerContext
    {
        public BaseCard Card
        {
            get; private set;
        }

        internal CardEnteredGraveyardContext(Game game, BaseCard card)
            : base(game)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            Card = card;
        }
    }
}
