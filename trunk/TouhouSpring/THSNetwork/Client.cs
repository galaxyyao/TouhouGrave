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

        public OutboxMessageQueue OutboxQueue
        {
            get;
            private set;
        }

        public InboxMessageQueue InboxQueue
        {
            get;
            private set;
        }

        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.AutoFlushSendQueue = false;
            _client = new NetClient(config);
            _client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            InitializeInboxActionLookups();
            InitializeOutboxActionLookups();
            OutboxQueue = new OutboxMessageQueue(this);
            InboxQueue = new InboxMessageQueue(this);
            NetworkStatus = NetworkStatusEnum.Disconnected;
        }

        public Game CurrentGame
        {
            get;
            set;
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
