using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    public class PostCardDrawnContext : TriggerContext
    {
        public BaseCard CardDrawn
        {
            get;
            private set;
        }

        internal PostCardDrawnContext(Game game, BaseCard cardDrawn)
            : base(game)
        {
            if (cardDrawn == null)
            {
                throw new ArgumentNullException("cardDrawn");
            }

            CardDrawn = cardDrawn;
        }
    }
}
