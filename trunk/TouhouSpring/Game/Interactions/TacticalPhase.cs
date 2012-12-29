﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class TacticalPhase : BaseInteraction
    {
        public enum Action
        {
            PlayCard,       // play a card (hero, assist, warrior or spell card)
            CastSpell,      // cast a spell from a warrior on battlefield
            Sacrifice,      // put one hand card to sacrifice zone
            Redeem,         // return one card from sacrifice to hand
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

        private BaseController Controller
        {
            get { return Player.Controller; }
        }

        public TacticalPhase(Player player)
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
            PlayCardCandidates = EnumeratePlayCardCandidates()
                                    .Where(card => player.Game.IsCardPlayable(card)).ToArray().ToIndexable();
            CastSpellCandidates = EnumerateCastSpellCandidates()
                                    .SelectMany(card => card.Spells)
                                    .Where(spell => player.Game.IsSpellCastable(spell)).ToArray().ToIndexable();
            SacrificeCandidates = player.CardsOnHand.Clone();
            RedeemCandidates = player.CardsSacrificed
                                    // TODO: check the mana cost
                                    //.Where(card => player.Game.IsCardPlayable(card))
                                    .ToArray().ToIndexable();
        }

        public Result Run()
        {
            var result = NotifyAndWait<Result>(Controller);
            Validate(result);
            return result;
        }

        public void Respond()
        {
            RespondBack(Controller, new Result { ActionType = Action.Pass });
        }

        public void Respond(BaseCard selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result { Data = selectedCard };
            if (PlayCardCandidates.Contains(selectedCard))
            {
                result.ActionType = Action.PlayCard;
            }
            else if (SacrificeCandidates.Contains(selectedCard))
            {
                result.ActionType = Action.Sacrifice;
            }
            else if (RedeemCandidates.Contains(selectedCard))
            {
                result.ActionType = Action.Redeem;
            }

            Validate(result);
            RespondBack(Controller, result);
        }

        public void Respond(Behaviors.ICastableSpell selectedSpell)
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
            RespondBack(Controller, result);
        }

        protected void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Action.Pass:
                    if (result.Data != null)
                    {
                        throw new InvalidDataException("Action Pass shall have null data.");
                    }
                    break;

                case Action.PlayCard:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InvalidDataException("Action PlayCard shall have an object of BaseCard as its data.");
                    }
                    else if (!PlayCardCandidates.Contains(result.Data))
                    {
                        throw new InvalidDataException("Selected card can't be played.");
                    }
                    break;

                case Action.CastSpell:
                    if (!(result.Data is Behaviors.ICastableSpell))
                    {
                        throw new InvalidDataException("Action CastSpell shall have an object of ICastableSpell as its data.");
                    }
                    if (!CastSpellCandidates.Contains(result.Data))
                    {
                        throw new InvalidDataException("Selected spell can't be casted.");
                    }
                    break;

                case Action.Sacrifice:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InvalidDataException("Action Sacrifice shall have an object of BaseCard as its data.");
                    }
                    else if (!SacrificeCandidates.Contains(result.Data))
                    {
                        throw new InvalidDataException("Selected card can't be sacrificed.");
                    }
                    break;

                case Action.Redeem:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InvalidDataException("Action Redeem shall have an object of BaseCard as its data.");
                    }
                    else if (!RedeemCandidates.Contains(result.Data))
                    {
                        throw new InvalidDataException("Selected card can't be redeemed.");
                    }
                    break;

                default:
                    throw new InvalidDataException("Invalid action performed.");
            }
        }

        private IEnumerable<BaseCard> EnumeratePlayCardCandidates()
        {
            if (!Player.CardsOnBattlefield.Contains(Player.Hero))
            {
                yield return Player.Hero;
            }
            foreach (var card in Player.Assists)
            {
                if (!Player.ActivatedAssists.Contains(card))
                {
                    yield return card;
                }
            }
            foreach (var card in Player.CardsOnHand)
            {
                yield return card;
            }
        }

        private IEnumerable<BaseCard> EnumerateCastSpellCandidates()
        {
            foreach (var card in Player.ActivatedAssists)
            {
                yield return card;
            }
            foreach (var card in Player.CardsOnBattlefield)
            {
                yield return card;
            }
        }
    }
}
