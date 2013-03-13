using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THSNetwork = TouhouSpring.Network;
using System.Threading;
using System.Configuration;

namespace TouhouSpring.Services
{
    public class Network : GameService
    {
        public THSNetwork.Client THSClient
        {
            get;
            private set;
        }

        public override void Startup()
        {
            THSClient = new THSNetwork.Client();
            THSClient.RemoteServerIp = ConfigurationManager.AppSettings["RemoteServerIp"].ToString();
            THSClient.RemoteServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["RemoteServerPort"].ToString());
        }

        public override void Shutdown()
        {
            THSClient = null;
        }
    }
}
