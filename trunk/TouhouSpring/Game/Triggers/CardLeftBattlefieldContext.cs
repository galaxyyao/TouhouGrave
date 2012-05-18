using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    public class CardLeftBattlefieldContext : TriggerContext
    {
        public BaseCard Card
        {
            get; private set;
        }

        internal CardLeftBattlefieldContext(Game game, BaseCard card)
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
