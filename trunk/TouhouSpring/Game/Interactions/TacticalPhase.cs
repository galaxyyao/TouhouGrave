using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public partial class TacticalPhase : BaseInteraction
    {
        public enum Action
        {
            PlayCard,       // play a card (hero, warrior or spell card)
            ActivateAssist, // activate an assist
            CastSpell,      // cast a spell from a warrior on battlefield
            Sacrifice,      // put one hand card to sacrifice zone
            Redeem,         // return one card from sacrifice to hand
            AttackCard,     // card attacks a opponent card
            AttackPlayer,   // card attacks the opponent player
            Pass,
            Abort
        }

        public struct Result
        {
            public Action ActionType;
            public object Data;
        }

        public Player Player
        {
            get; private set;
        }

        public IIndexable<CardInstance> PlayCardCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> ActivateAssistCandidates
        {
            get; private set;
        }

        public IIndexable<Behaviors.ICastableSpell> CastSpellCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> SacrificeCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> RedeemCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> AttackerCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> DefenderCandidates
        {
            get; private set;
        }

        public bool CanPass
        {
            get; private set;
        }

        internal TacticalPhase(Player player)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (player != player.Game.ActingPlayer)
            {
                throw new InvalidOperationException("TacticalPhase can only be invoked on the acting player.");
            }

            Player = player;
            PlayCardCandidates = GetPlayCardBaseSet(player)
                                    .Where(card => player.Game.IsCardPlayable(card)).ToArray().ToIndexable();
            ActivateAssistCandidates = GetActivateAssistBaseSet(player)
                                    .Where(card => player.Game.IsCardActivatable(card)).ToArray().ToIndexable();
            CastSpellCandidates = GetCastSpellBaseSet(player).SelectMany(card => card.Spells)
                                    .Where(spell => player.Game.IsSpellCastable(spell)).ToArray().ToIndexable();
            SacrificeCandidates = !Game.DidSacrifice ? player.CardsOnHand.Clone() : Indexable.Empty<CardInstance>();
            RedeemCandidates = !Game.DidRedeem
                               ? player.CardsSacrificed.Where(card => player.Game.IsCardRedeemable(card)).ToArray().ToIndexable()
                               : Indexable.Empty<CardInstance>();
            AttackerCandidates = GetAttackerBaseSet(player)
                                    .Where(card => !card.Behaviors.Has<Behaviors.Neutralize>()).ToArray().ToIndexable();
            if (AttackerCandidates.Count != 0)
            {
                var defenders = GetDefenderBaseSet(player).ToArray();
                var protectors = defenders.Where(card => card.Behaviors.Has<Behaviors.Protector>()).ToArray();
                DefenderCandidates = (protectors.Length != 0 ? protectors : defenders).ToIndexable();
            }
            else
            {
                DefenderCandidates = Indexable.Empty<CardInstance>();
            }

            CanPass = !AttackerCandidates.Any(card => card.Behaviors.Has<Behaviors.ForceAttack>());
            Game.CurrentInteraction = this;
        }

        internal Result Run()
        {
            var result = NotifyAndWait<Result>();
            Validate(result);
            return result;
        }

        public void RespondPass()
        {
            var result = new Result
            {
                ActionType = Action.Pass
            };
            Validate(result);
            RespondBack(result);
        }

        public void RespondPlay(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Action.PlayCard,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondActivate(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Action.ActivateAssist,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondCast(Behaviors.ICastableSpell selectedSpell)
        {
            if (selectedSpell == null)
            {
                throw new ArgumentNullException("selectedSpell");
            }

            var result = new Result
            {
                ActionType = Action.CastSpell,
                Data = selectedSpell
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondSacrifice(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Action.Sacrifice,
                Data = selectedCard
            };
            
            Validate(result);
            RespondBack(result);
        }

        public void RespondRedeem(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Action.Redeem,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAttackCard(CardInstance attacker, CardInstance defender)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (defender == null)
            {
                throw new ArgumentNullException("defender");
            }

            var result = new Result
            {
                ActionType = Action.AttackCard,
                Data = new CardInstance[] { attacker, defender }
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAttackPlayer(CardInstance attacker, Player player)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            var result = new Result
            {
                ActionType = Action.AttackPlayer,
                Data = new object[] { attacker, player }
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAbort()
        {
            RespondBack(new Result { ActionType = Action.Abort });
        }

        protected void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Action.Pass:
                    if (!CanPass)
                    {
                        throw new InteractionValidationFailException("Can't pass.");
                    }
                    else if (result.Data != null)
                    {
                        throw new InteractionValidationFailException("Action Pass shall have null data.");
                    }
                    break;

                case Action.PlayCard:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action PlayCard shall have an object of CardInstance as its data.");
                    }
                    else if (!PlayCardCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be played.");
                    }
                    break;

                case Action.ActivateAssist:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action ActivateAssist shall have an object of CardInstance as its data.");
                    }
                    else if (!ActivateAssistCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected assist card can't be activated.");
                    }
                    break;

                case Action.CastSpell:
                    if (!(result.Data is Behaviors.ICastableSpell))
                    {
                        throw new InteractionValidationFailException("Action CastSpell shall have an object of ICastableSpell as its data.");
                    }
                    if (!CastSpellCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected spell can't be casted.");
                    }
                    break;

                case Action.Sacrifice:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action Sacrifice shall have an object of CardInstance as its data.");
                    }
                    else if (!SacrificeCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be sacrificed.");
                    }
                    break;

                case Action.Redeem:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action Redeem shall have an object of CardInstance as its data.");
                    }
                    else if (!RedeemCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be redeemed.");
                    }
                    break;

                case Action.AttackCard:
                    {
                        var pair = result.Data as CardInstance[];
                        if (pair == null || pair.Length != 2)
                        {
                            throw new InteractionValidationFailException("Action AttackCard shall have a pair of CardInstances as its data.");
                        }
                        else if (!AttackerCandidates.Contains(pair[0]))
                        {
                            throw new InteractionValidationFailException("Attacking card can't attack.");
                        }
                        else if (!DefenderCandidates.Contains(pair[1]))
                        {
                            throw new InteractionValidationFailException("Defending card can't defend.");
                        }
                    }
                    break;

                case Action.AttackPlayer:
                    {
                        var pair = result.Data as object[];
                        if (pair == null || pair.Length != 2 || !(pair[0] is CardInstance) || !(pair[1] is Player))
                        {
                            throw new InteractionValidationFailException("Action AttackPlayer shall have a pair of CardInstance and Player as its data.");
                        }
                        else if (!AttackerCandidates.Contains(pair[0]))
                        {
                            throw new InteractionValidationFailException("Attacking card can't attack.");
                        }
                        else if (pair[1] == Player)
                        {
                            throw new InteractionValidationFailException("Player can't be attacked.");
                        }
                    }
                    break;

                case Action.Abort:
                    break;

                default:
                    throw new InteractionValidationFailException("Invalid action performed.");
            }
        }

        private static IEnumerable<CardInstance> GetPlayCardBaseSet(Player player)
        {
            if (!player.CardsOnBattlefield.Contains(player.Hero) && player.Hero != null)
            {
                yield return player.Hero;
            }
            foreach (var card in player.CardsOnHand)
            {
                yield return card;
            }
        }

        private static IEnumerable<CardInstance> GetActivateAssistBaseSet(Player player)
        {
            foreach (var card in player.Assists)
            {
                if (!player.ActivatedAssits.Contains(card))
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<CardInstance> GetCastSpellBaseSet(Player player)
        {
            foreach (var card in player.ActivatedAssits)
            {
                yield return card;
            }
            foreach (var card in player.CardsOnBattlefield)
            {
                yield return card;
            }
        }

        private static IEnumerable<CardInstance> GetAttackerBaseSet(Player player)
        {
            foreach (var card in player.CardsOnBattlefield)
            {
                var warrior = card.Behaviors.Get<Behaviors.Warrior>();
                if (warrior != null && warrior.State == Behaviors.WarriorState.StandingBy)
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<CardInstance> GetDefenderBaseSet(Player player)
        {
            foreach (var p in player.Game.Players)
            {
                if (p != player)
                {
                    foreach (var card in p.CardsOnBattlefield)
                    {
                        if (card.Behaviors.Has<Behaviors.Warrior>())
                        {
                            yield return card;
                        }
                    }
                }
            }
        }
    }
}
