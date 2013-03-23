using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using System.Diagnostics;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private NetClient _client;

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

        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.AutoFlushSendQueue = false;
            _client = new NetClient(config);
            _client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            InitializeInteractionActionsTable();
            InitializeOutboxActionTable();
            NetworkStatus = NetworkStatusEnum.Disconnected;
        }

        public enum NetworkStatusEnum
        {
            Connecting, ResendingConnect, Connected, Disconnected, ConnectFailed
        }

        public NetworkStatusEnum NetworkStatus
        {
            get;
            private set;
        }
    }
}
