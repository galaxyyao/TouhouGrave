using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    // in this simulator, mana-consuming actions are done
    class MainStage1Simulator : BaseSimulator
    {
        private struct CardIntPair
        {
            public BaseCard Card;
            public int Int;
        }

        private class CardModelComparer : IEqualityComparer<CardIntPair>
        {
            public bool Equals(CardIntPair p1, CardIntPair p2)
            {
                return p1.Card.Model == p2.Card.Model;
            }

            public int GetHashCode(CardIntPair p)
            {
                return p.Card.Model.GetHashCode();
            }
        }

        private bool m_stopActivateAssists = false;

        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            foreach (var indexedCard in io.SacrificeCandidates
                                            .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                            .Distinct(new CardModelComparer()))
            {
                yield return new SacrificeChoice(indexedCard.Int);
            }

            foreach (var indexedCard in io.PlayCardCandidates
                                            .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                            .Distinct(new CardModelComparer()))
            {
                yield return new PlayCardChoice(indexedCard.Int);
            }

            foreach (var indexedCard in io.RedeemCandidates
                                            .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                            .Distinct(new CardModelComparer()))
            {
                yield return new RedeemChoice(indexedCard.Int);
            }

            if (!m_stopActivateAssists)
            {
                for (int i = 0; i < io.ActivateAssistCandidates.Count; ++i)
                {
                    yield return new ActivateAssistChoice(i);
                }
                m_stopActivateAssists = true;
            }

            for (int i = 0; i < io.CastSpellCandidates.Count; ++i)
            {
                yield return new CastSpellChoice(i);
            }

            yield return new PassChoice();
        }

        public override IEnumerable<Choice> SelectCards(Interactions.SelectCards io, Context context)
        {
            for (int i = 0; i < io.SelectFromSet.Count; ++i)
            {
                yield return new SelectCardChoice(i);
            }
        }
    }
}
