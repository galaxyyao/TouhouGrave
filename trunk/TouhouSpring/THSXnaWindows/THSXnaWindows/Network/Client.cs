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

        private int _roomId = 0;

        public enum RoomStatusEnum
        {
            Waiting, Starting, Started, End
        }

        public RoomStatusEnum RoomStatus
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
                                //TODO: Show "Connected" information on UI
                                ;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //TODO: Show "Lost Connection" Information on UI
                                //TODO: Abort Game Thread and return to Main Menu
                                ;
                            }

                            string text = status.ToString() + im.ReadString();
                            Debug.Print(text);
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Debug.Print(text);
                            InterpretMessage(text);
                        }
                        break;
                    default:
                        Debug.Print("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        private string InterpretMessage(string message)
        {
            List<string> parts = message.Split(' ').ToList();
            switch (parts[1])
            {
                case "enterroom":
                    {
                        _roomId = Convert.ToInt32(parts[0]);
                        return string.Format("{0} {1}", _roomId, "roomentered");
                    }
                case "startgame":
                    {
                        RoomStatus = RoomStatusEnum.Starting;
                        return string.Format("{0} {1}", _roomId, "gamestarted");
                    }
                default:
                    Debug.Print("Invalid argument");
                    break;
            }
            return null;
        }

        public int GetRoomId()
        {
            return _roomId;
        }
    }
}
