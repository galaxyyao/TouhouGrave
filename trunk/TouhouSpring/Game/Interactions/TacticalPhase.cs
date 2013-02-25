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
            Pass
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

        public IIndexable<BaseCard> PlayCardCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> ActivateAssistCandidates
        {
            get; private set;
        }

        public IIndexable<Behaviors.ICastableSpell> CastSpellCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> SacrificeCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> RedeemCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> AttackerCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> DefenderCandidates
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
            SacrificeCandidates = !Game.DidSacrifice ? player.CardsOnHand.Clone() : Indexable.Empty<BaseCard>();
            RedeemCandidates = !Game.DidRedeem
                               ? player.CardsSacrificed.Where(card => player.Game.IsCardRedeemable(card)).ToArray().ToIndexable()
                               : Indexable.Empty<BaseCard>();
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
                DefenderCandidates = Indexable.Empty<BaseCard>();
            }
        }

        internal Result Run()
        {
            var result = NotifyAndWait<Result>();
            Validate(result);
            return result;
        }

        public void RespondPass()
        {
            RespondBack(new Result { ActionType = Action.Pass });
        }

        public void RespondPlay(BaseCard selectedCard)
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
            Game.CurrentCommand.ResultIndex = PlayCardCandidates.IndexOf(selectedCard);
            RespondBack(result);
        }

        public void RespondActivate(BaseCard selectedCard)
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
            Game.CurrentCommand.ResultIndex = ActivateAssistCandidates.IndexOf(selectedCard);
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
            Game.CurrentCommand.ResultIndex = CastSpellCandidates.IndexOf(selectedSpell);
            RespondBack(result);
        }

        public void RespondSacrifice(BaseCard selectedCard)
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
            Game.CurrentCommand.ResultIndex = SacrificeCandidates.IndexOf(selectedCard);
            RespondBack(result);
        }

        public void RespondRedeem(BaseCard selectedCard)
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
            Game.CurrentCommand.ResultIndex = RedeemCandidates.IndexOf(selectedCard);
            RespondBack(result);
        }

        public void RespondAttackCard(BaseCard attacker, BaseCard defender)
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
                Data = new BaseCard[] { attacker, defender }
            };

            Validate(result);
            Game.CurrentCommand.ResultIndex = AttackerCandidates.IndexOf(attacker);
            Game.CurrentCommand.ResultIndex2 = DefenderCandidates.IndexOf(defender);
            RespondBack(result);
        }

        public void RespondAttackPlayer(BaseCard attacker, Player player)
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
            Game.CurrentCommand.ResultIndex = AttackerCandidates.IndexOf(attacker);
            RespondBack(result);
        }

        protected void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Action.Pass:
                    if (result.Data != null)
                    {
                        throw new InteractionValidationFailException("Action Pass shall have null data.");
                    }
                    break;

                case Action.PlayCard:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InteractionValidationFailException("Action PlayCard shall have an object of BaseCard as its data.");
                    }
                    else if (!PlayCardCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be played.");
                    }
                    break;

                case Action.ActivateAssist:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InteractionValidationFailException("Action ActivateAssist shall have an object of BaseCard as its data.");
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
                    if (!(result.Data is BaseCard))
                    {
                        throw new InteractionValidationFailException("Action Sacrifice shall have an object of BaseCard as its data.");
                    }
                    else if (!SacrificeCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be sacrificed.");
                    }
                    break;

                case Action.Redeem:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InteractionValidationFailException("Action Redeem shall have an object of BaseCard as its data.");
                    }
                    else if (!RedeemCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be redeemed.");
                    }
                    break;

                case Action.AttackCard:
                    {
                        var pair = result.Data as BaseCard[];
                        if (pair == null || pair.Length != 2)
                        {
                            throw new InteractionValidationFailException("Action AttackCard shall have a pair of BaseCards as its data.");
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
                        if (pair == null || pair.Length != 2 || !(pair[0] is BaseCard) || !(pair[1] is Player))
                        {
                            throw new InteractionValidationFailException("Action AttackPlayer shall have a pair of BaseCard and Player as its data.");
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

                default:
                    throw new InteractionValidationFailException("Invalid action performed.");
            }
        }

        private static IEnumerable<BaseCard> GetPlayCardBaseSet(Player player)
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

        private static IEnumerable<BaseCard> GetActivateAssistBaseSet(Player player)
        {
            foreach (var card in player.Assists)
            {
                if (!player.ActivatedAssits.Contains(card))
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<BaseCard> GetCastSpellBaseSet(Player player)
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

        private static IEnumerable<BaseCard> GetAttackerBaseSet(Player player)
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

        private static IEnumerable<BaseCard> GetDefenderBaseSet(Player player)
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
