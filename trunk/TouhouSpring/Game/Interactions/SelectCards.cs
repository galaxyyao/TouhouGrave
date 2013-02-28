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

        public IIndexable<BaseCard> Candidates
        {
            get; private set;
        }

        public SelectMode Mode
        {
            get; private set;
        }

        public SelectCards(Player player, IIndexable<BaseCard> candidates, SelectMode mode)
            : this(player, candidates, mode, null)
        { }

        public SelectCards(Player player, IIndexable<BaseCard> candidates, SelectMode mode, string message)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (candidates == null || candidates.Count == 0)
            {
                throw new ArgumentNullException("candidates");
            }

            Player = player;
            Candidates = candidates;
            Message = message ?? String.Empty;
            Mode = mode;
        }

        public virtual IIndexable<BaseCard> Run()
        {
            var result = NotifyAndWait<IIndexable<BaseCard>>();
            Validate(result);
            return result;
        }

        public virtual void Respond(IIndexable<BaseCard> selectedCards)
        {
            Validate(selectedCards);
            Game.CurrentCommand.ResultParameters=new int[selectedCards.Count];
            int i=0;
            foreach (var selectedCard in selectedCards)
            {
                Game.CurrentCommand.ResultParameters[i] = Candidates.IndexOf(selectedCard);
                i++;
            }
            RespondBack(selectedCards);
        }

        protected void Validate(IIndexable<BaseCard> selectedCards)
        {
            if (selectedCards == null)
            {
                throw new InteractionValidationFailException("selectedCards");
            }
            else if (Mode == SelectMode.Single && selectedCards.Count != 0 && selectedCards.Count != 1)
            {
                throw new InteractionValidationFailException("Only one or zero card can be selected.");
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
