using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent
    {
        private BaseCard Sacrifice_MakeChoice(Interactions.TacticalPhase io)
        {
            int currentManaCost = io.Player.Mana;
            var sacrificeCandidates = io.SacrificeCandidates.Where(card =>
                card.Behaviors.Has<Behaviors.ManaCost>()
                && card.Behaviors.Get<Behaviors.ManaCost>().Cost > currentManaCost + 1).ToArray();
            return sacrificeCandidates.Length != 0
                   ? sacrificeCandidates[new Random().Next(sacrificeCandidates.Length)]
                   : null;
        }

        private BaseCard Sacrifice_MakeChoice2(Interactions.TacticalPhase io)
        {
            var sacrificeScore = io.SacrificeCandidates
                .Select(card => new CardScorePair { Card = card, Score = EvaluateSacrifceScore(card, io) })
                .OrderByDescending(csp => csp.Score).ToArray();
            var selection = sacrificeScore.FirstOrDefault();

            // mana pool size
            double manaPoolSizeScore = 1.0 / (io.Player.MaxMana + 1);
            // hand set count
            double handSetCount = io.Player.CardsOnHand.Count;

            double finalScore = selection.Score * manaPoolSizeScore * handSetCount;

            // TODO: decide the final threshold value
            return finalScore < 1 ? null : selection.Card;
        }

        private double EvaluateSacrifceScore(BaseCard card, Interactions.TacticalPhase io)
        {
            // mana cost
            var manaCostScore = card.Behaviors.Has<Behaviors.ManaCost>()
                                ? Math.Abs(card.Behaviors.Get<Behaviors.ManaCost>().Cost - io.Player.Mana - 1)
                                : 0;

            // TODO: sacrifice value

            return manaCostScore;
        }
    }
}
