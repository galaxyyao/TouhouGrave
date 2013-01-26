using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent
    {
        private Dictionary<string, int> m_scoreLibrary = new Dictionary<string, int>();

        // TODO: save/load score library

        private int GetScore(ICardModel cardModel)
        {
            int score;
            if (m_scoreLibrary.TryGetValue(cardModel.Id, out score))
            {
                return score;
            }

            var initialScore = GetInitialScore(cardModel);
            m_scoreLibrary.Add(cardModel.Id, initialScore);
            return initialScore;
        }

        private int GetInitialScore(ICardModel cardModel)
        {
            var manaCost = cardModel.Behaviors.FirstOrDefault(bm => bm is Behaviors.ManaCost.ModelType);
            if (manaCost == null)
            {
                return 0;
            }
            else
            {
                return (manaCost as Behaviors.ManaCost.ModelType).Cost * 10;
            }
        }
    }
}
