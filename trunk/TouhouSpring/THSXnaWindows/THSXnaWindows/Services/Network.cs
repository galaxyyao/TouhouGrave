using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THSNetwork = TouhouSpring.Network;
using System.Threading;
using System.Configuration;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(CurrentProfile))]
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
            THSClient.RemoteServerIp = AppSettings.Instance.RemoteServerIp;
            THSClient.RemoteServerPort = AppSettings.Instance.RemoteServerPort;
        }

        public override void Shutdown()
        {
            THSClient = null;
        }
    }
}
