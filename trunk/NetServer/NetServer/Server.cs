using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace TouhouSpring.NetServerCore
{
    public partial class Server
    {
        private readonly int _port;

        private const int s_maxConnNum = 100;

        private NetServer _server;

        private NetIncomingMessage _im; 

        public bool IsRunning
        {
            get
            {
                if (_server == null)
                    return false;
                return _server.Status == NetPeerStatus.Running;
            }
        }

        public int Status
        {
            get
            {
                return (int)_server.Status;
            }
        }

        public Server(int port)
        {
            _port = port;
        }

    }
}
