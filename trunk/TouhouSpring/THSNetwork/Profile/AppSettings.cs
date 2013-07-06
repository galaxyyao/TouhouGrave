using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TouhouSpring
{
    [XmlRoot]
    public class AppSettings
    {
        [XmlElement("GameSettings")]
        public GameSettings GameSettings
        {
            get;
            set;
        }

        [XmlElement("Profiles")]
        public Profiles Profiles
        {
            get;
            set;
        }

        public AppSettings()
        {
            GameSettings = new GameSettings();
            Profiles = new Profiles();
        }
    }
}
