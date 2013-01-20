using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// A preselected set of cards.
    /// </summary>
    public class Deck : IIndexable<ICardModel>
    {
        public enum ValidationResult
        {
            Okay,
            NoHero,                 // no card selected as hero
            InvalidAssist,          // non-assist card selected as assist
            TooManyAssists,         // number of assists bigger than 3
            IdenticalAssists,       // one assist repeats
            TooLessCards,           // number of cards less than 50
            TooManyIdenticalCards,  // number of one group of identical cards bigger than 3
            InvalidCard,            // invalid card selected (hero and assist)
        }

        private List<ICardModel> m_cards = new List<ICardModel>();

        public ICardModel this[int index]
        {
            get { return m_cards[index]; }
        }

        public ICardModel Hero
        {
            get; set;
        }

        public IList<ICardModel> Assists
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        public int Count
        {
            get { return m_cards.Count; }
        }

        public int IndexOf(ICardModel element)
        {
            return m_cards.IndexOf(element);
        }

        public Deck(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
            Assists = new List<ICardModel>();
        }

        public ValidationResult Validate()
        {
            /*if (Hero == null)
            {
                return ValidationResult.NoHero;
            }
            else*/ if (Assists.Any(card => !card.Behaviors.Any(bhvMdl => bhvMdl is Behaviors.Assist.ModelType)))
            {
                return ValidationResult.InvalidAssist;
            }
            else if (Assists.Count > 3)
            {
                return ValidationResult.TooManyAssists;
            }
            else if (Assists.Distinct().Count() != Assists.Count)
            {
                return ValidationResult.IdenticalAssists;
            }
            // TODO: uncomment the next block when the card database is big enough
            //else if (Count < 50)
            //{
            //    return ValidationResult.TooLessCards;
            //}
            else if (m_cards.GroupBy(card => card).Any(grp => grp.Count() > 3))
            {
                return ValidationResult.TooManyIdenticalCards;
            }
            else if (m_cards.Any(card => card == Hero || card.Behaviors.Any(bhvMdl => bhvMdl is Behaviors.Assist.ModelType)))
            {
                return ValidationResult.InvalidCard;
            }

            return ValidationResult.Okay;
        }

        public void Add(ICardModel cardModel)
        {
            if (cardModel == null)
            {
                throw new ArgumentNullException("cardModel");
            }
            m_cards.Add(cardModel);
        }

        public IEnumerator<ICardModel> GetEnumerator()
        {
            return m_cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_cards.GetEnumerator();
        }
    }
}
