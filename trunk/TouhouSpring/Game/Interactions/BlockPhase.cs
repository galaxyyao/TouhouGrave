using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class BlockPhase : BaseInteraction
    {
        public BaseController Controller
        {
            get; private set;
        }

        public IIndexable<BaseCard> DeclaredAttackers
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
            BlockerCandidates = GetBlockerCandidates().ToArray().ToIndexable();
            PlayableCandidates = GetPlayableCandidates().ToArray().ToIndexable();
        }

        public object Run()
        {
            var result = NotifyAndWait<object>(Controller);
            if (result is BaseCard)
            {
                Validate((BaseCard)result);
            }
            else if (result is IIndexable<IIndexable<BaseCard>>)
            {
                Validate((IIndexable<IIndexable<BaseCard>>)result);
            }
            return result;
        }

        public void Respond(BaseCard cardToPlay)
        {
            Validate(cardToPlay);
            RespondBack(Controller, cardToPlay);
        }

        public void Respond(IIndexable<IIndexable<BaseCard>> blockers)
        {
            Validate(blockers);
            RespondBack(Controller, blockers);
        }

        private IEnumerable<BaseCard> GetBlockerCandidates()
        {
            return Controller.Player.CardsOnBattlefield.Where(card => card.Behaviors.Has<Behaviors.Warrior>() && card.State == CardState.StandingBy);
        }

        private IEnumerable<BaseCard> GetPlayableCandidates()
        {
            return Controller.Player.CardsOnHand.Where(card => !card.Behaviors.OfType<Behaviors.IPlayable>().Any(p => !p.IsPlayable(Controller.Game)));
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
            else
            {
                // check uniqueness of blockers
                for (int i = 0; i < blockers.Count; ++i)
                {
                    var b = blockers[i];
                    if (!b.Unique()
                        || blockers.AnyFrom(i + 1, bb => b.Count > 0 && bb.Contains(b[0]) || b.Count > 1 && bb.Contains(b[1])))
                    {
                        throw new InvalidDataException("One blocker can only be declared to block one attacker.");
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
