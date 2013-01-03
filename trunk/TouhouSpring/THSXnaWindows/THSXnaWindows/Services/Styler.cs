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
		private XDocument m_document;

		public XElement GetCardStyle(string styleId)
		{
			if (styleId == null)
			{
				throw new ArgumentNullException("styleId");
			}

			return m_document.Root.Elements("Card").First(elem => elem.Attribute("Id").Value == styleId);
		}

		public XElement GetPageStyle(string pageId)
		{
			if (pageId == null)
			{
				throw new ArgumentNullException("pageId");
			}

			return m_document.Root.Elements("Page").First(elem => elem.Attribute("Id").Value == pageId);
		}

        public XElement GetPlayerZonesStyle()
        {
            return m_document.Root.Elements("PlayerZones").First();
        }

		public override void Startup()
		{
			m_document = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.UIStyles.xml"));
		}

		public override void Shutdown()
		{
			m_document = null;
		}
	}
}
