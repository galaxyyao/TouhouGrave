using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public class GameSettings
    {
        public string RemoteServerIp
        {
            get;
            set;
        }

        public int RemoteServerPort
        {
            get;
            set;
        }

        public bool IsMusicOn
        {
            get;
            set;
        }

        public bool IsSoundOn
        {
            get;
            set;
        }

        public float MusicVolume
        {
            get;
            set;
        }

        public float SoundVolume
        {
            get;
            set;
        }
    }
}
