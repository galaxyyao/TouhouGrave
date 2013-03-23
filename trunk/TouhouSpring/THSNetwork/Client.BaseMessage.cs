using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Diagnostics;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private List<string> _messageQueue = new List<string>();

        public void Connect()
        {
            _client.Start();
            _client.Connect(RemoteServerIp, RemoteServerPort);
        }

        public void Connect(string host, int port)
        {
            RemoteServerIp = host;
            RemoteServerPort = port;
            Connect();
            NetworkStatus = NetworkStatusEnum.Connecting;
        }

        public void Disconnect()
        {
            _client.Disconnect("Disconnect request by user");
            _client.Shutdown("bye");
            NetworkStatus = NetworkStatusEnum.Disconnected;
        }

        public void SendMessage(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
            Debug.Print(string.Format("{0} Sent:{1}", _client.UniqueIdentifier, text));
        }

        private void GotMessage(object peer)
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
                            Debug.Print(im.MessageType.ToString() + "-" + text);
                            if (text == "Resending Connect...")
                                NetworkStatus = NetworkStatusEnum.ResendingConnect;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string text = status.ToString() + im.ReadString();
                            Debug.Print(im.MessageType.ToString() + "-" + text);

                            if (status == NetConnectionStatus.Connected)
                            {
                                NetworkStatus = NetworkStatusEnum.Connected;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //TODO: Show "Lost Connection" Information on UI
                                //TODO: Abort Game Thread and return to Main Menu
                                NetworkStatus = NetworkStatusEnum.ConnectFailed;
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Debug.Print(string.Format("{0} Received:{1}", _client.UniqueIdentifier, text));
                            var parts = text.Split(' ');
                            if (parts[1] == "enterroom")
                            {
                                RoomId = Convert.ToInt32(parts[0]);
                                Seed = -1;
                                SendMessage(string.Format("{0} {1}", RoomId, "roomentered"));
                            }
                            else if (parts[1] == "waiting")
                            {
                            }
                            else if (parts[1] == "disconnect")
                            {
                            }
                            else if (parts[1] == "startgame")
                            {
                                RoomStatus = RoomStatusEnum.Starting;
                                StartupIndex = Convert.ToInt32(parts[2]);
                                SendMessage(string.Format("{0} {1}", RoomId, "gamestarted"));
                            }
                            else if (parts[1] == "generateseed")
                            {
                                Seed = Convert.ToInt32(parts[2]);
                            }
                            else
                            {
                                OnInteractionMessageArrive(parts);
                            }
                        }
                        break;
                    default:
                        Debug.Print("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }
    }
}
