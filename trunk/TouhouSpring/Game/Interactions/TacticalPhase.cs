using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class TacticalPhase : SelectCards
    {
        public enum Action
        {
            DrawCard,
            PlayCard,
            CastSpell,
            Skip
        }

        public struct Result
        {
            public Action ActionType;
            public object Data;
        }

        public TacticalPhase(BaseController controller)
            : base(controller, ComputeFromSet(controller).ToArray().ToIndexable(), SelectMode.Single,
                   "Select a card from hand to play onto the battlefield or cast a spell from a card on battlefield.")
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (controller != controller.Game.PlayerController)
            {
                throw new InvalidOperationException("TacticalPhase can only be invoked on the current player.");
            }
        }

        public new Result Run()
        {
            var result = NotifyAndWait<Result>(Controller);
            Validate(result);
            return result;
        }

        public override void Respond(IIndexable<BaseCard> selectedCards)
        {
            bool doSkip = selectedCards == null || selectedCards.Count == 0;
            var result = new Result
            {
                ActionType = doSkip ? Action.Skip : Action.PlayCard,
                Data = doSkip ? null : selectedCards[0]
            };

            Validate(result);
            RespondBack(Controller, result);
        }

        public void Respond(BaseCard selectedCard)
        {
            var result = new Result
            {
                ActionType = selectedCard == null ? Action.Skip : Action.PlayCard,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(Controller, result);
        }

        public void Respond(Behaviors.ICastableSpell selectedSpell)
        {
            var result = new Result
            {
                ActionType = selectedSpell == null ? Action.Skip : Action.CastSpell,
                Data = selectedSpell
            };

            Validate(result);
            RespondBack(Controller, result);
        }

        public void RespondDraw()
        {
            RespondBack(Controller, new Result { ActionType = Action.DrawCard });
        }

        public IIndexable<BaseCard> ComputeCastFromSet()
        {
            return Controller.Player.CardsOnBattlefield.Where(card =>
                card.Behaviors.Has<Behaviors.Warrior>()
                && card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.StandingBy).ToArray().ToIndexable();
        }

        protected void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Action.Skip:
                    if (result.Data != null)
                    {
                        throw new InvalidDataException("Action Skip shall have null data.");
                    }
                    break;

                case Action.DrawCard:
                    if (result.Data != null)
                    {
                        throw new InvalidDataException("Action DrawCard shall have null data.");
                    }
                    break;

                case Action.PlayCard:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InvalidDataException("Action PlayCard shall have an object of BaseCard as its data.");
                    }
                    base.Validate(new BaseCard[] { (BaseCard)result.Data }.ToIndexable());
                    break;

                case Action.CastSpell:
                    if (!(result.Data is Behaviors.ICastableSpell))
                    {
                        throw new InvalidDataException("Action PlayCard shall have an object of ICastableSpell as its data.");
                    }
                    if (!ComputeCastFromSet().Contains(((Behaviors.ICastableSpell)result.Data).Host))
                    {
                        throw new InvalidDataException("Selected spell doesn't come from a card from player's battlefield.");
                    }
                    break;

                default:
                    throw new InvalidDataException("Invalid action performed.");
            }
        }

        private static IEnumerable<BaseCard> ComputeFromSet(BaseController controller)
        {
            return GetFromSet(controller.Player).Where(card => !card.Behaviors.OfType<Behaviors.IPlayable>().Any(p => !p.IsPlayable(controller.Game)));
        }

        private static IEnumerable<BaseCard> GetFromSet(Player player)
        {
            foreach (var card in player.CardsOnHand)
            {
                yield return card;
            }

            if (!player.CardsOnBattlefield.Contains(player.Hero.Host))
            {
                // hero card is on battlefield
                yield return player.Hero.Host;
            }
        }
    }
}
