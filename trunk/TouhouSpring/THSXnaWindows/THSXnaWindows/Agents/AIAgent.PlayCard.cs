using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent
    {
        private BaseCard PlayOrActivateCard_MakeChoice(Interactions.TacticalPhase io)
        {
            var playSelection = io.PlayCardCandidates
                .Select(card => new CardScorePair { Card = card, Score = EvaluatePlayScore(card) });
            var activateSelection = io.ActivateAssistCandidates
                .Select(card => new CardScorePair { Card = card, Score = EvaluatePlayScore(card) });
            var selection = playSelection.Concat(activateSelection).OrderByDescending(csp => csp.Score).FirstOrDefault();
            return selection.Card;
        }

        private double EvaluatePlayScore(BaseCard card)
        {
            var score = GetScore(card.Model);
            
            // TODO: play card value adjustment
            var adjustment = 0;

            return score + adjustment;
        }
    }
}
