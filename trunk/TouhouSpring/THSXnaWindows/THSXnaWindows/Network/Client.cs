using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private NetClient _client;

        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.AutoFlushSendQueue = false;
            _client = new NetClient(config);
            _client.RegisterReceivedCallback(new SendOrPostCallback(GotCommand));
        }

        public void Connect(string host, int port)
        {
        }

        public void GotCommand(object peer)
        {
        }
    }
}
