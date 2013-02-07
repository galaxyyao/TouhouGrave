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

        public object Command
        {
            get;
            set;
        }

        public int RoomId
        {
            get;
            private set;
        }

        public enum RoomStatusEnum
        {
            Waiting, Starting, Started, End
        }

        public RoomStatusEnum RoomStatus
        {
            get;
            set;
        }

        public int Seed
        {
            get;
            private set;
        }

        public int StartupIndex
        {
            get;
            private set;
        }

        public Interactions.BaseInteraction CurrentIo
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
        }
    }
}
