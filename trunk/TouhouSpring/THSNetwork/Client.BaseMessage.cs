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
            _client.Connect(RemoteServerIp,RemoteServerPort);
        }

        public void Connect(string host, int port)
        {
            RemoteServerIp = host;
            RemoteServerPort = port;
            Connect();
        }

        public void Disconnect()
        {
            _client.Disconnect("Disconnect request by user");
            _client.Shutdown("bye");
        }

        public void SendMessage(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
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
                            Debug.Print(text);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string text = status.ToString() + im.ReadString();
                            Debug.Print(text);

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
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Debug.Print(text);
                            IntepretMessage(text);
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
