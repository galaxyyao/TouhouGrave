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

        private int RoomNum = 0;

        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.AutoFlushSendQueue = false;
            _client = new NetClient(config);
            //_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
        }

        public void Connect(string host, int port)
        {
            _client.Start();
            _client.Connect(host, port);
        }

        public void Disconnect()
        {
            _client.Disconnect("Disconnect request by user");
            _client.Shutdown("bye");
        }

        public void Send(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
        }

        public void GotMessage(object peer)
        {
        }
    }
}
