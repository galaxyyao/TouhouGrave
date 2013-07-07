using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TouhouSpring
{
    /// <summary>
    /// A preselected set of cards.
    /// </summary>
    public class Deck
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



        [XmlIgnore]
        public ICardModel this[int index]
        {
            get { return Cards[index]; }
        }

        [XmlIgnore]
        public ICardModel Hero
        {
            get;
            set;
        }

        [XmlIgnore]
        public IList<ICardModel> Assists
        {
            get;
            private set;
        }

        [XmlIgnore]
        public IList<ICardModel> Cards
        {
            get;
            private set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        [XmlElement("Card")]
        public CardIdList DeckCardIdList
        {
            get;
            set;
        }

        [XmlElement("Assist")]
        public AssistIdList DeckAssistIdList
        {
            get;
            set;
        }

        [XmlIgnore]
        public int Count
        {
            get { return Cards.Count; }
        }

        public int IndexOf(ICardModel element)
        {
            return Cards.IndexOf(element);
        }

        public Deck()
        {
            Assists = new List<ICardModel>();
            Cards = new List<ICardModel>();
        }

        public ValidationResult Validate()
        {
            /*if (Hero == null)
            {
                return ValidationResult.NoHero;
            }
            else*/
            if (Assists.Any(card => !card.Behaviors.Any(bhvMdl => bhvMdl is Behaviors.Assist.ModelType)))
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
            else if (Cards.GroupBy(card => card).Any(grp => grp.Count() > 3))
            {
                return ValidationResult.TooManyIdenticalCards;
            }
            else if (Cards.Any(card => card == Hero || card.Behaviors.Any(bhvMdl => bhvMdl is Behaviors.Assist.ModelType)))
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
            Cards.Add(cardModel);
        }

        public IEnumerator<ICardModel> GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return m_cards.GetEnumerator();
        //}
    }

    public class CardIdList
    {
        [XmlElement("Model")]
        public List<string> Model
        {
            get;
            set;
        }
    }

    public class AssistIdList
    {
        [XmlElement("Model")]
        public List<string> Model
        {
            get;
            set;
        }
    }
}
