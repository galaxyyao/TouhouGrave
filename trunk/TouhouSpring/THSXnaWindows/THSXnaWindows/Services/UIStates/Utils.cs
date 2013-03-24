using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    static class Utils
    {
        public static bool Contains(this IEnumerable<CardInstance> cardArray, int cardGuid)
        {
            foreach (var card in cardArray)
            {
                if (card.Guid == cardGuid)
                {
                    return true;
                }
            }

            return false;
        }

        public static CardInstance Find(this IEnumerable<CardInstance> cardArray, int cardGuid)
        {
            foreach (var card in cardArray)
            {
                if (card.Guid == cardGuid)
                {
                    return card;
                }
            }

            return null;
        }

        public static CardInstance[] MapToCards(this List<int> cardGuidArray, IIndexable<CardInstance> candidates)
        {
            var cardArray = new CardInstance[cardGuidArray.Count];
            for (int i = 0; i < cardGuidArray.Count; ++i)
            {
                cardArray[i] = candidates.Find(cardGuidArray[i]);
            }
            return cardArray;
        }
    }
}
