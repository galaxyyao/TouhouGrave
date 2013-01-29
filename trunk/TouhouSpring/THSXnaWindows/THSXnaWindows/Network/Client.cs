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

        private int RoomNum = 0;

        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.AutoFlushSendQueue = false;
            _client = new NetClient(config);
            _client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
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
            NetIncomingMessage im;
            while ((im = _client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            string text = im.ReadString();
                            //Output(text);
                            Debug.Print(text);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                            if (status == NetConnectionStatus.Connected)
                            {
                                ;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                ;
                            }

                            string text = im.ReadString();
                            //Output(status.ToString() + ": " + reason);
                            Debug.Print(text);
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            //Output(chat);
                            Debug.Print(text);
                        }
                        break;
                    default:
                        //Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        Debug.Print("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }
    }
}
