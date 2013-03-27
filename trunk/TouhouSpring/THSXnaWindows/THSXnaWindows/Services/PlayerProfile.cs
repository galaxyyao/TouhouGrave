using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    public class PlayerProfile : GameService
    {
        private Profile m_playerProfile;

        class Profile
        {

        }

        public override void Startup()
        {
            XmlDocument profile = new XmlDocument();
            profile.Load("Profile.xml");
            System.Diagnostics.Debug.Print(profile.InnerXml);
        }
    }
}
