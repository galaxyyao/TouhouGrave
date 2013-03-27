using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace TouhouSpring
{
    public class AppSettings
    {
        private static AppSettings m_instance;

        private string m_ip;
        private int m_port;
        private bool m_isMusicOn;
        private bool m_isSoundOn;
        private float m_musicVolume;
        private float m_soundVolume;

        public static AppSettings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AppSettings();
                }
                return m_instance;
            }
        }

        private XDocument CurrentProfile
        {
            get;
            set;
        }

        public string RemoteServerIp
        {
            get
            {
                return m_ip;
            }
            private set
            {
                m_ip = value;
            }
        }

        public int RemoteServerPort
        {
            get
            {
                return m_port;
            }
            private set
            {
                m_port = value;
            }
        }

        public bool IsMusicOn
        {
            get
            {
                return m_isMusicOn;
            }
            set
            {
                m_isMusicOn = value;
                WriteSetting("IsMusicOn", value.ToString());
            }
        }

        public bool IsSoundOn
        {
            get
            {
                return m_isSoundOn;
            }
            set
            {
                m_isSoundOn = value;
                WriteSetting("IsSoundOn", value.ToString());
            }
        }

        public float MusicVolume
        {
            get
            {
                return m_musicVolume;
            }
            set
            {
                m_musicVolume = value;
                WriteSetting("MusicVolume", value.ToString());
            }
        }

        public float SoundVolume
        {
            get
            {
                return m_soundVolume;
            }
            set
            {
                m_soundVolume = value;
                WriteSetting("SoundVolume", value.ToString());
            }
        }

        private AppSettings()
        {
            CurrentProfile = XDocument.Load("Profile.xml");
            ReadSettings();
        }

        private void ReadSettings()
        {
            m_ip = (from seg in CurrentProfile.Root.Descendants("RemoteServerIp")
                             select (string)seg).FirstOrDefault();
            if (m_ip == null)
            {
                throw new InvalidDataException("ip cannot be null");
            }
            if (!Int32.TryParse((from seg in CurrentProfile.Root.Descendants("RemoteServerPort")
                                 select (string)seg).FirstOrDefault(), out m_port))
                throw new InvalidDataException("Invalid port");
            if (!Boolean.TryParse((from seg in CurrentProfile.Root.Descendants("IsMusicOn")
                                 select (string)seg).FirstOrDefault(), out m_isMusicOn))
                throw new InvalidDataException("Invalid isMusicOn");
            if (!Boolean.TryParse((from seg in CurrentProfile.Root.Descendants("IsSoundOn")
                                   select (string)seg).FirstOrDefault(), out m_isSoundOn))
                throw new InvalidDataException("Invalid isSoundOn");
            if (!float.TryParse((from seg in CurrentProfile.Root.Descendants("MusicVolume")
                                   select (string)seg).FirstOrDefault(), out m_musicVolume))
                throw new InvalidDataException("Invalid MusicVolume");
            if (!float.TryParse((from seg in CurrentProfile.Root.Descendants("SoundVolume")
                                 select (string)seg).FirstOrDefault(), out m_soundVolume))
                throw new InvalidDataException("Invalid SoundVolume");
        }

        private void WriteSetting(string elementName, string value)
        {
            XElement element = (from seg in CurrentProfile.Root.Descendants(elementName)
                                select seg).FirstOrDefault();
            element.Value = value;
            CurrentProfile.Save("Profile.xml", SaveOptions.None);
        }

        public void ReadProfile()
        {

        }
    }
}
