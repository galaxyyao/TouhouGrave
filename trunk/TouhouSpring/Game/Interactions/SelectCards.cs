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
            Single,			// select 0-1 card
            Multiple		// select 0-N cards, N is the number of cards at the specified location
        }

        public Player Player
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public IIndexable<BaseCard> SelectFromSet
        {
            get; private set;
        }

        public SelectMode Mode
        {
            get; private set;
        }

        protected BaseController Controller
        {
            get { return Player.Controller; }
        }

        public SelectCards(Player player, IIndexable<BaseCard> fromSet, SelectMode mode)
            : this(player, fromSet, mode, null)
        { }

        public SelectCards(Player player, IIndexable<BaseCard> fromSet, SelectMode mode, string message)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
            SelectFromSet = fromSet;
            Message = message ?? String.Empty;
            Mode = mode;
        }

        public virtual IIndexable<BaseCard> Run()
        {
            var result = NotifyAndWait<IIndexable<BaseCard>>(Controller);
            Validate(result);
            return result;
        }

        public virtual void Respond(IIndexable<BaseCard> selectedCards)
        {
            Validate(selectedCards);
            RespondBack(Controller, selectedCards);
        }

        protected void Validate(IIndexable<BaseCard> selectedCards)
        {
            if (selectedCards == null)
            {
                throw new ArgumentNullException("selectedCards");
            }
            else if (Mode == SelectMode.Single && selectedCards.Count != 0 && selectedCards.Count != 1)
            {
                throw new InvalidDataException("Only one or zero card can be selected.");
            }
            else if (!selectedCards.Unique())
            {
                throw new InvalidDataException("The selected cards has duplicate.");
            }
            else if (selectedCards.Any(card => !SelectFromSet.Contains(card)))
            {
                throw new InvalidDataException("Some of the cards can't be selected.");
            }
        }
    }
}
