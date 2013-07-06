using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TouhouSpring
{
    public partial class Profiles
    {
        [XmlIgnore]
        public Profile CurrentProfile
        {
            get;
            set;
        }

        [XmlElement("Profile")]
        public List<Profile> ProfileList
        {
            get;
            set;
        }

        public Profiles()
        {
            ProfileList = new List<Profile>();
        }
    }
}
