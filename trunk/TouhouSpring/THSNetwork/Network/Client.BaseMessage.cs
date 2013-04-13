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
                            string message = im.ReadString();
                            Debug.Print(im.MessageType.ToString() + message);
                            if (message == "Resending Connect...")
                                NetworkStatus = NetworkStatusEnum.ResendingConnect;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string message = im.ReadString();
                            Debug.Print(im.MessageType.ToString() + message);

                            if (status == NetConnectionStatus.Connected)
                            {
                                NetworkStatus = NetworkStatusEnum.Connected;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                OnInteractionMessageArrived(message);
                                NetworkStatus = NetworkStatusEnum.ConnectFailed;
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string message = im.ReadString();
                            OnInteractionMessageArrived(message);
                            Debug.Print(im.MessageType.ToString() + message);
                        }
                        break;
                    default:
                        Debug.Print("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        public void StartRandomGame()
        {
            SendMessage(string.Format(
                "<Message><Type>Game</Type><Time>{0}</Time><Info><Action>StartRandomGame</Action></Info></Message>"
                , DateTime.Now));
        }
    }
}
