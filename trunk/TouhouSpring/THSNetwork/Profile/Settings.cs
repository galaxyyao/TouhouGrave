using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public class Settings
    {
        private static Settings m_instance;
        private const string ProfileFilePath = "Profile.xml";

        public static Settings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new Settings();
                }
                return m_instance;
            }
        }

        public AppSettings MyAppSettings
        {
            get;
            set;
        }

        public Settings()
        {
            MyAppSettings = new AppSettings();
        }

        public void LoadSettings()
        {
            MyAppSettings = ExtSerialization.XmlDeserializeFromFilePath<AppSettings>(ProfileFilePath);
            MyAppSettings.Profiles.CurrentProfile =
                (from profile in MyAppSettings.Profiles.ProfileList
                 where profile.Uid == "default"
                 select profile).FirstOrDefault();
        }

        public void SaveSettings()
        {
            ExtSerialization.XmlSerializeToXml<AppSettings>(MyAppSettings, ProfileFilePath);
        }
    }
}
