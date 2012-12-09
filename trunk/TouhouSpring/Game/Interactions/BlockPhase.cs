using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class BlockPhase : BaseInteraction
    {
        public enum Action
        {
            ConfirmBlock,
            PlayCard
        }

        public struct Result
        {
            public Action ActionType;
            public object Data;
        }

        public BaseController Controller
        {
            get; private set;
        }

        public IIndexable<BaseCard> DeclaredAttackers
        {
            get; private set;
        }

        public IEnumerable<BaseCard> BlockableAttackers
        {
            get; private set;
        }

        public IIndexable<BaseCard> BlockerCandidates
        {
            get; private set;
        }

        public IIndexable<BaseCard> PlayableCandidates
        {
            get; private set;
        }

        public BlockPhase(BaseController controller, IIndexable<BaseCard> declaredAttackers)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (declaredAttackers == null)
            {
                throw new ArgumentNullException("declaredAttackers");
            }

            Controller = controller;
            DeclaredAttackers = declaredAttackers;
            BlockableAttackers = declaredAttackers.Where(card => !card.Behaviors.Has<Behaviors.Unblockable>());
            BlockerCandidates = GetBlockerCandidates().ToArray().ToIndexable();
            PlayableCandidates = GetPlayableCandidates().ToArray().ToIndexable();
        }

        public Result Run()
        {
            var result = NotifyAndWait<Result>(Controller);
            Validate(result);
            return result;
        }

        public void Respond(IIndexable<IIndexable<BaseCard>> blockers)
        {
            var result = new Result
            {
                ActionType = Action.ConfirmBlock,
                Data = blockers
            };

            Validate(result);
            RespondBack(Controller, result);
        }

        public void Respond(BaseCard cardToPlay)
        {
            var result = new Result
            {
                ActionType = Action.PlayCard,
                Data = cardToPlay
            };

            Validate(result);
            RespondBack(Controller, result);
        }

        private IEnumerable<BaseCard> GetBlockerCandidates()
        {
            return Controller.Player.CardsOnBattlefield.Where(card =>
                card.Behaviors.Has<Behaviors.Warrior>()
                && card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.StandingBy
                && !card.Behaviors.Has<Behaviors.NonBlocker>());
        }

        private IEnumerable<BaseCard> GetPlayableCandidates()
        {
            return Controller.Player.CardsOnHand.Where(card => Controller.Game.IsCardPlayable(card));
        }

        private void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Action.ConfirmBlock:
                    if (!(result.Data is IIndexable<IIndexable<BaseCard>>))
                    {
                        throw new InvalidDataException("Action ConfirmBlock shall have an array of arrays of BaseCard as its data.");
                    }
                    Validate(result.Data as IIndexable<IIndexable<BaseCard>>);
                    break;

                case Action.PlayCard:
                    if (!(result.Data is BaseCard))
                    {
                        throw new InvalidDataException("Action PlayCard shall have an object of BaseCard as its data.");
                    }
                    Validate(result.Data as BaseCard);
                    break;

                default:
                    throw new InvalidDataException("Invalid action performed.");
            }
        }

        private void Validate(IIndexable<IIndexable<BaseCard>> blockers)
        {
            if (blockers == null)
            {
                throw new ArgumentNullException("blockers");
            }
            else if (blockers.Count != DeclaredAttackers.Count)
            {
                throw new InvalidDataException("Blockers array's length doesn't match attackers array's.");
            }
            else if (blockers.Contains(null))
            {
                throw new InvalidDataException("Blockers array can't contain null element.");
            }
            else if (blockers.Any(b => b.Count > 2))
            {
                throw new InvalidDataException("Two blockers at most can be declared on one single attacker.");
            }
            else if (blockers.Any(b => b.Any(bb => !bb.Behaviors.Has<Behaviors.Warrior>())))
            {
                throw new InvalidDataException("Blockers must be warriors.");
            }
            else if (blockers.Any(b => b.Any(bb => bb.Behaviors.Has<Behaviors.NonBlocker>())))
            {
                throw new InvalidDataException("NonBlocker can't block other card.");
            }
            else
            {
                for (int i = 0; i < blockers.Count; ++i)
                {
                    // check uniqueness of blockers
                    var b = blockers[i];
                    if (!b.Unique()
                        || blockers.AnyFrom(i + 1, bb => b.Count > 0 && bb.Contains(b[0]) || b.Count > 1 && bb.Contains(b[1])))
                    {
                        throw new InvalidDataException("One blocker can only be declared to block one attacker.");
                    }

                    // check unblockable
                    if (DeclaredAttackers[i].Behaviors.Has<Behaviors.Unblockable>()
                        && b.Count > 0)
                    {
                        throw new InvalidDataException("Unblockable attacker can't be blocked.");
                    }
                }
            }
        }

        private void Validate(BaseCard cardToPlay)
        {
            if (cardToPlay == null)
            {
                throw new ArgumentNullException("cardToPlay");
            }
            else if (!PlayableCandidates.Contains(cardToPlay))
            {
                throw new ArgumentException(String.Format("{0} can't be played."), cardToPlay.Model.Name);
            }
        }
    }
}
