using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class SelectCards : BaseInteraction
    {
        public enum SelectMode
        {
            Single,     // select 0-1 card
            Multiple    // select 0-N cards, N is the number of cards at the specified location
        }

        public Player Player
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public IIndexable<CardInstance> Candidates
        {
            get; private set;
        }

        public SelectMode Mode
        {
            get; private set;
        }

        public SelectCards(Player player, IEnumerable<CardInstance> candidates, SelectMode mode)
            : this(player, candidates, mode, null)
        { }

        public SelectCards(Player player, IEnumerable<CardInstance> candidates, SelectMode mode, string message)
            : this(player, candidates.Where(card => !card.Behaviors.Has<Behaviors.IUnselectable>()).ToArray().ToIndexable(), mode, message)
        { }

        internal SelectCards(Player player, IIndexable<CardInstance> filteredCandidates, SelectMode mode, string message)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (filteredCandidates == null)
            {
                throw new ArgumentNullException("candidates");
            }

            Player = player;
            Candidates = filteredCandidates;
            Message = message ?? String.Empty;
            Mode = mode;
        }

        public virtual IIndexable<CardInstance> Run()
        {
            var result = Candidates.Count != 0
                         ? NotifyAndWait<IIndexable<CardInstance>>()
                         : null;
            Validate(result);
            return result;
        }

        public virtual void Respond(IIndexable<CardInstance> selectedCards)
        {
            Validate(selectedCards);
            RespondBack(selectedCards);
        }

        protected void Validate(IIndexable<CardInstance> selectedCards)
        {
            if (selectedCards == null)
            {
                return;
            }

            if (selectedCards.Count == 0)
            {
                throw new InteractionValidationFailException("Zero-length array is not allowed.");
            }
            else if (Mode == SelectMode.Single && selectedCards.Count > 1)
            {
                throw new InteractionValidationFailException("Only one card can be selected.");
            }
            else if (!selectedCards.Unique())
            {
                throw new InteractionValidationFailException("The selected cards has duplicate.");
            }
            else if (selectedCards.Any(card => !Candidates.Contains(card)))
            {
                throw new InteractionValidationFailException("Some of the cards can't be selected.");
            }
        }
    }
}
