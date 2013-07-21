using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Services
{
    class Styler : GameService
    {
        private XDocument m_cardDesignsDoc;
        private XDocument m_cardStylesDoc;
        private XDocument m_pagesDoc;
        private XDocument m_playerZonesDoc;

        public XElement GetCardStyle(string styleId)
        {
            if (styleId == null)
            {
                throw new ArgumentNullException("styleId");
            }

            return m_cardStylesDoc.Root.Elements("CardStyle").First(elem => elem.Attribute("Id").Value == styleId);
        }

        public XElement GetCardDesign(string designId)
        {
            if (designId == null)
            {
                throw new ArgumentNullException("designId");
            }

            return m_cardDesignsDoc.Root.Elements("CardDesign").First(elem => elem.Attribute("Name").Value == designId);
        }

        public XElement GetPageStyle(string pageId)
        {
            if (pageId == null)
            {
                throw new ArgumentNullException("pageId");
            }

            return m_pagesDoc.Root.Elements("Page").First(elem => elem.Attribute("Id").Value == pageId);
        }

        public XElement GetPlayerZonesStyle()
        {
            return m_playerZonesDoc.Root;
        }

        public override void Startup()
        {
            m_cardDesignsDoc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.StyleDefs.CardDesigns.xml"));
            m_cardStylesDoc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.StyleDefs.CardStyles.xml"));
            m_pagesDoc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.StyleDefs.Pages.xml"));
            m_playerZonesDoc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.StyleDefs.PlayerZones.xml"));
        }

        public override void Shutdown()
        {
            m_playerZonesDoc = null;
            m_pagesDoc = null;
            m_cardStylesDoc = null;
            m_cardDesignsDoc = null;
        }
    }
}
