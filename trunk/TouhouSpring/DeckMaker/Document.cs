using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TouhouSpring;
using System.Xml.Linq;

namespace TouhouSpring
{
    class Document
    {
        public enum DocType
        {
            CardLibrary, Deck
        }

        public string DeckFileName
        {
            get;
            private set;
        }

        public List<DeckMaker.CardModel> CardLibraryCards
        {
            get;
            private set;
        }

        public Dictionary<string, XElement> DeckProfileElements
        {
            get;
            private set;
        }

        public Dictionary<string, XElement> DeckElements
        {
            get;
            private set;
        }

        public List<DeckMaker.CardModel> DeckCards
        {
            get;
            private set;
        }

        private XDocument m_deckDoc;
        private string m_deckPath;

        public Document()
        {
        }

        public void Open(string fileName, DocType docType)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName is null");
            }
            if (docType == DocType.Deck)
            {
                DeckFileName = fileName;
            }
            try
            {
                XDocument doc = XDocument.Load(fileName);
                if (docType == DocType.CardLibrary)
                {
                    LoadCardLibrary(doc);
                }
                else if (docType == DocType.Deck)
                {
                    m_deckDoc = doc;
                    m_deckPath = fileName;
                    LoadDeckProfile(doc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to load xml document, exception message:{0}", ex.Message));
            }
        }

        private void LoadCardLibrary(XDocument doc)
        {
            List<XElement> cardElements = doc.Descendants("Resources").FirstOrDefault().Elements().ToList();
            CardLibraryCards = new List<DeckMaker.CardModel>();
            foreach (var cardElement in cardElements)
            {
                DeckMaker.CardModel card = new DeckMaker.CardModel
                {
                    Id = ExtXML.GetFirstDescendantsValue<string, XElement>(cardElement, "Id"),
                    Name = ExtXML.GetFirstDescendantsValue<string, XElement>(cardElement, "Name"),
                    Description = ExtXML.GetFirstDescendantsValue<string, XElement>(cardElement, "Description"),
                    ArtworkUri = ExtXML.GetFirstDescendantsValue<string, XElement>(cardElement, "ArtworkUri")
                };

                List<XElement> behaviorElements = cardElement.Descendants("Behaviors").FirstOrDefault().Elements().ToList();
                XElement manaElement = (from el in behaviorElements
                                        where el.Attribute("Type").Value == "TouhouSpring.Behaviors.ManaCost+ModelType"
                                        select el).FirstOrDefault();
                if (manaElement != null)
                    card.ManaCost = ExtXML.GetFirstDescendantsValue<Int32, XElement>(manaElement, "Cost");
                XElement assistElement = (from el in behaviorElements
                                          where el.Attribute("Type").Value == "TouhouSpring.Behaviors.Assist+ModelType"
                                          select el).FirstOrDefault();
                if (assistElement != null)
                    card.CardType = DeckMaker.CardModel.CardTypeEnum.Assist;
                XElement warriorElement = (from el in behaviorElements
                                           where el.Attribute("Type").Value == "TouhouSpring.Behaviors.Warrior+ModelType"
                                           select el).FirstOrDefault();
                if (warriorElement != null)
                {
                    card.CardType = DeckMaker.CardModel.CardTypeEnum.Warrior;
                    card.Attack = ExtXML.GetFirstDescendantsValue<Int32, XElement>(warriorElement, "Attack");
                    card.Life = ExtXML.GetFirstDescendantsValue<Int32, XElement>(warriorElement, "Life");
                }
                XElement spellElement = (from el in behaviorElements
                                         where el.Attribute("Type").Value == "TouhouSpring.Behaviors.Instant+ModelType"
                                         select el).FirstOrDefault();
                if (spellElement != null)
                    card.CardType = DeckMaker.CardModel.CardTypeEnum.Spell;

                CardLibraryCards.Add(card);
            }
        }

        private void LoadDeckProfile(XDocument doc)
        {
            List<XElement> profiles = doc.Descendants("Profiles").FirstOrDefault().Elements().ToList();
            DeckProfileElements = new Dictionary<string, XElement>();
            foreach (XElement deckProfileElement in profiles)
            {
                string profileName = ExtXML.GetFirstDescendantsValue<string, XElement>(deckProfileElement, "Uid");
                DeckProfileElements.Add(profileName, deckProfileElement);
            }
        }

        public void LoadDeck(string profileName)
        {
            XElement deckProfileElement = DeckProfileElements[profileName];
            List<XElement> decks = deckProfileElement.Element("Decks").Elements().ToList();
            DeckElements = new Dictionary<string, XElement>();
            foreach (XElement deckElement in decks)
            {
                string deckName = ExtXML.GetFirstDescendantsValue<string, XElement>(deckElement, "Name");
                DeckElements.Add(deckName, deckElement);
            }
        }

        public void LoadDeckCards(string deckName)
        {
            XElement deckElement = DeckElements[deckName];
            List<XElement> cardWithoutAssistElements = deckElement.Element("Card").Elements().ToList();
            List<XElement> assistElements = deckElement.Element("Assist").Elements().ToList();
            DeckCards = new List<DeckMaker.CardModel>();
            foreach (XElement cardElement in cardWithoutAssistElements)
            {
                string cardName = cardElement.Value;
                DeckMaker.CardModel libraryCard = (from card in CardLibraryCards
                                                   where card.Id == cardName
                                                   select card).FirstOrDefault();
                DeckCards.Add((DeckMaker.CardModel)libraryCard.Clone());
            }
            foreach (XElement cardElement in assistElements)
            {
                string cardName = cardElement.Value;
                DeckMaker.CardModel libraryCard = (from card in CardLibraryCards
                                                   where card.Id == cardName
                                                   select card).FirstOrDefault();
                DeckCards.Add((DeckMaker.CardModel)libraryCard.Clone());
            }
        }

        public void Save(string profileName, string decName)
        {
            XElement oldDeckElement = DeckElements[decName];
            string deckId = ExtXML.GetFirstDescendantsValue<string, XElement>(oldDeckElement, "Id");
            string deckName = ExtXML.GetFirstDescendantsValue<string, XElement>(oldDeckElement, "Name");
            XElement newDeckElement =
                new XElement("Deck",
                    new XElement("Id", deckId),
                    new XElement("Name", deckName),
                    new XElement("Card"),
                    new XElement("Assist")
                    );
            foreach (var card in DeckCards)
            {
                if (card.CardType == DeckMaker.CardModel.CardTypeEnum.Assist)
                {
                    newDeckElement.Element("Assist").Add(new XElement("Model", card.Id));
                }
                if (card.CardType == DeckMaker.CardModel.CardTypeEnum.Spell
                    || card.CardType == DeckMaker.CardModel.CardTypeEnum.Warrior)
                {
                    newDeckElement.Element("Card").Add(new XElement("Model", card.Id));
                }
            }
            oldDeckElement.ReplaceWith(newDeckElement);
            m_deckDoc.Save(m_deckPath);
        }
    }
}
