using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    // in this simulator, mana-consuming actions are done
    class MainPhaseSimulator : BaseSimulator
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

        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, IContext context)
        {
            // sacrifice
            if (context.CurrentBranchOrder <= 1)
            {
                var redundantGroup = io.SacrificeCandidates
                    .Select((card, index) => new CardIntPair { Card = card, Int = index })
                    .GroupBy(cip => cip.Card.Model).Where(g => g.Count() > 1).FirstOrDefault();
                if (redundantGroup != null)
                {
                    var indexedCard = redundantGroup.First();
                    yield return new SacrificeChoice(indexedCard.Int, indexedCard.Card.Model)
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name }
#endif
                    ;
                }
                else
                {
                    foreach (var indexedCard in io.SacrificeCandidates
                                                    .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                                    .Distinct(new CardModelComparer()))
                    {
                        yield return new SacrificeChoice(indexedCard.Int, indexedCard.Card.Model)
#if DEBUG
                        { DebugName = indexedCard.Card.Model.Name }
#endif
                        ;
                    }
                }
            }

            // redeem
            if (context.CurrentBranchOrder <= 2)
            {
                var lastSacrificeChoice = context.CurrentBranchChoicePath.LastOrDefault(choice => choice is SacrificeChoice);
                var lastSacrificedCardModel = lastSacrificeChoice != null ? (lastSacrificeChoice as SacrificeChoice).CardModel : null;
                foreach (var indexedCard in io.RedeemCandidates
                                            .Select((card, index) => new CardIntPair { Card = card, Int = index })
                                            .Distinct(new CardModelComparer())
                                            .Where(ic => ic.Card.Model != lastSacrificedCardModel))
                {
                    yield return new RedeemChoice(indexedCard.Int, indexedCard.Card.Guid)
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name }
#endif
                    ;
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
            // cast spell
            if (context.CurrentBranchOrder <= 4)
            {
                // when play multiple warriors in one turn, we don't treat "Play A then B" and "Play B then A" as two
                // different paths by sorting the warriors by their guid
                var distinctCardsOnHand = io.PlayCardCandidates
                    .Select((card, index) => new CardIntPair { Card = card, Int = index })
                    .Distinct(new CardModelComparer()).ToArray();
                var lastPlayedWarrior = context.CurrentBranchChoicePath.LastOrDefault(choice => choice is PlayCardChoice
                    && (choice as PlayCardChoice).IsWarrior);
                if (lastPlayedWarrior != null)
                {
                    var lastPlayedWarriorGuid = (lastPlayedWarrior as PlayCardChoice).CardGuid;
                    distinctCardsOnHand = distinctCardsOnHand.Where(ic => ic.Card.Guid > lastPlayedWarriorGuid).ToArray();
                }

                foreach (var indexedCard in distinctCardsOnHand)
                {
                    yield return new PlayCardChoice(
                        indexedCard.Int,
                        indexedCard.Card.Guid,
                        indexedCard.Card.Behaviors.Has<Behaviors.Warrior>())
#if DEBUG
                    { DebugName = indexedCard.Card.Model.Name }
#endif
                    ;
                }

                for (int i = 0; i < io.CastSpellCandidates.Count; ++i)
                {
                    yield return new CastSpellChoice(i);
                }
            }

            // kill branch if redeemed card is not played
            var redeemChoice = context.CurrentBranchChoicePath.FirstOrDefault(choice => choice is RedeemChoice) as RedeemChoice;
            if (redeemChoice != null)
            {
                if (!context.CurrentBranchChoicePath.Any(choice =>
                    choice is PlayCardChoice
                    && (choice as PlayCardChoice).CardGuid == redeemChoice.CardGuid))
                {
                    yield return new KillBranchChoice();
                    yield break;
                }
            }

            // attack card
            // attack player
            if (context.CurrentBranchOrder <= 5)
            {
                if (io.DefenderCandidates.Count != 0)
                {
                    // the last attacked card has a priority to be attacked again
                    var lastAttackCardChoice = context.CurrentBranchChoicePath.LastOrDefault() as AttackCardChoice;
                    var lastAttackedCardIndex = lastAttackCardChoice != null
                        ? io.DefenderCandidates.FindIndex(card => card.Guid == lastAttackCardChoice.DefenderGuid)
                        : -1;
                    if (lastAttackedCardIndex != -1)
                    {
                        for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                        {
                            yield return new AttackCardChoice(i, lastAttackedCardIndex, lastAttackCardChoice.DefenderGuid);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                        {
                            for (int j = 0; j < io.DefenderCandidates.Count; ++j)
                            {
                                yield return new AttackCardChoice(i, j, io.DefenderCandidates[j].Guid);
                            }
                        }
                    }
                }
                // when attack the opponent player we don't care about the attacking order
                else if (io.AttackerCandidates.Count > 0)
                {
                    for (int j = 0; j < io.Game.Players.Count(); ++j)
                    {
                        if (io.Game.ActingPlayerEnemies.Contains(io.Game.Players[j]))
                        {
                            yield return new AttackPlayerChoice(0, j);
                        }
                    }
                }
            }

            if (io.CanPass)
            {
                yield return new PassChoice();
            }
        }

        public override IEnumerable<Choice> SelectCards(Interactions.SelectCards io, IContext context)
        {
            for (int i = 0; i < io.Candidates.Count; ++i)
            {
                yield return new SelectCardChoice(i)
#if DEBUG
                { DebugName = io.Candidates[i].Model.Name }
#endif
                ;
            }
        }

        public override IEnumerable<Choice> SelectNumber(Interactions.SelectNumber io, IContext context)
        {
            for (int i = io.Minimum; i <= io.Maximum; ++i)
            {
                yield return new SelectNumberChoice(i);
            }
        }
    }
}
