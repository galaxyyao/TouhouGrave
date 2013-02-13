using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    // in this simulator, mana-consuming actions are done
    class MainPhaseSimulator : BaseSimulator<Context>
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

        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            // redeem
            if (context.CurrentBranchOrder <= 1)
            {
                foreach (var indexedCard in io.RedeemCandidates
                                            .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                            .Distinct(new CardModelComparer()))
                {
                    yield return new RedeemChoice(indexedCard.Int);
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name };
#endif
                }
            }

            // sacrifice
            if (context.CurrentBranchOrder <= 2)
            {
                foreach (var indexedCard in io.SacrificeCandidates
                                                .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                                .Distinct(new CardModelComparer()))
                {
                    yield return new SacrificeChoice(indexedCard.Int);
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name };
#endif
                }
            }

            // activate assist
            // ActivateAssistChoice has an order of 4, thus yielding once causes this step to be skipped for the rest
            // of the branch, making this choice to be only made once for one branch
            if (context.CurrentBranchOrder <= 3)
            {
                for (int i = 0; i < io.ActivateAssistCandidates.Count; ++i)
                {
                    yield return new ActivateAssistChoice(i);
                }
            }

            // play card
            // activate assist
            // cast spell
            if (context.CurrentBranchOrder <= 4)
            {
                foreach (var indexedCard in io.PlayCardCandidates
                                                .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                                .Distinct(new CardModelComparer()))
                {
                    yield return new PlayCardChoice(indexedCard.Int);
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name };
#endif
                }

                

                for (int i = 0; i < io.CastSpellCandidates.Count; ++i)
                {
                    yield return new CastSpellChoice(i);
                }
            }

            // attack card
            // attack player
            if (context.CurrentBranchOrder <= 5)
            {
                if (io.DefenderCandidates.Count != 0)
                {
                    for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                    {
                        for (int j = 0; j < io.DefenderCandidates.Count; ++j)
                        {
                            yield return new AttackCardChoice(i, j);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                    {
                        for (int j = 0; j < io.Game.Players.Count(); ++j)
                        {
                            if (io.Game.ActingPlayerEnemies.Contains(io.Game.Players[j]))
                            {
                                yield return new AttackPlayerChoice(i, j);
                            }
                        }
                    }
                }
            }

            yield return new PassChoice();
        }

        public override IEnumerable<Choice> SelectCards(Interactions.SelectCards io, Context context)
        {
            for (int i = 0; i < io.Candidates.Count; ++i)
            {
                yield return new SelectCardChoice(i);
#if DEBUG
                { DebugName = io.Candidates[i].Model.Name };
#endif
            }
        }
    }
}
