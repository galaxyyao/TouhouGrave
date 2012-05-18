using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private bool TriggerGlobal<TContext>(TContext context)
            where TContext : Triggers.TriggerContext
        {
            return TriggerGlobal(context, ctx => false, EnumerateRelevantCards().ToArray());
        }

        private bool TriggerGlobal<TContext>(TContext context, Predicate<TContext> earlyQuitPredicate)
            where TContext : Triggers.TriggerContext
        {
            return TriggerGlobal(context, earlyQuitPredicate, EnumerateRelevantCards().ToArray());
        }

        private bool TriggerGlobal<TContext>(TContext context, BaseCard[] relevantCards)
            where TContext : Triggers.TriggerContext
        {
            return TriggerGlobal(context, ctx => false, relevantCards);
        }

        private bool TriggerGlobal<TContext>(TContext context, Predicate<TContext> earlyQuitPredicate, BaseCard[] relevantCards)
            where TContext : Triggers.TriggerContext
        {
            foreach (var card in relevantCards)
            {
                if (!card.Trigger(context, earlyQuitPredicate))
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<BaseCard> EnumerateRelevantCards()
        {
            foreach (var player in Players)
            {
                foreach (var card in player.CardsOnHand)
                {
                    yield return card;
                }

                foreach (var card in player.CardsOnBattlefield)
                {
                    yield return card;
                }

                if (!player.CardsOnBattlefield.Contains(player.Hero.Host))
                {
                    yield return player.Hero.Host;
                }
            }
        }
    }
}
