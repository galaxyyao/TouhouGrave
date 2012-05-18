using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace TouhouSpring
{
    class CardFactory
    {
        private CardModelList m_cards;

        public ICardModel GetCardModel(string id)
        {
            return m_cards.FirstOrDefault(cm => cm.Id == id);
        }

        public CardFactory(string xml)
        {
            using (XmlReader xr = XmlReader.Create(xml))
            {
                m_cards = IntermediateSerializer.Deserialize<CardModelList>(xr, Path.GetDirectoryName(xml));
            }
        }
    }
}
