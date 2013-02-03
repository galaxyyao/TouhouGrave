using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace TouhouSpring.NetServerCore
{
    public partial class Server
    {
        public void Start()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ths");
            config.Port = _port;
            config.MaximumConnections = s_maxConnNum;

            _server = new NetServer(config);
            _server.Start();
        }

        public void Listen()
        {
            NetIncomingMessage im;
            while ((im = _server.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            string text = im.ReadString();
                            Console.WriteLine(text);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string text = im.ReadString();
                            Console.WriteLine(string.Format("{0} - {1}:{2}", NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier), status, text));

                            if (status == NetConnectionStatus.Connected)
                            {
                                int enteredRoomId = UserEnter(im.SenderConnection);
                                NetOutgoingMessage om = _server.CreateMessage();
                                om.Write(string.Format("{0} enterroom", enteredRoomId));
                                _server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                                if (enteredRoomId == -1)//Only 1 player in the room, waiting for another user
                                {
                                    om = _server.CreateMessage();
                                    om.Write(string.Format("{0} waiting", enteredRoomId));
                                    _server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                                else
                                {
                                    Room currentRoom = _roomList[enteredRoomId];
                                    om = _server.CreateMessage();
                                    om.Write(string.Format("{0} startgame", enteredRoomId));
                                    _server.SendMessage(om, currentRoom.Player1Connection, NetDeliveryMethod.ReliableOrdered, 0);
                                    om = _server.CreateMessage();
                                    om.Write(string.Format("{0} startgame", enteredRoomId));
                                    _server.SendMessage(om, currentRoom.Player2Connection, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //TODO:
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Console.WriteLine(text);
                            //TODO: Do something to Data
                            NetOutgoingMessage om = _server.CreateMessage();
                            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + "aaa");
                            _server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        break;
                    default:
                        Console.WriteLine(string.Format("Unhandled type: {0} {1} bytes {2}|{3}"
                            , im.MessageType
                            , im.LengthBytes
                            , im.DeliveryMethod
                            , im.SequenceChannel
                            ));
                        break;
                }
                Thread.Sleep(1);
            }
        }

        private string InterpretMessage(string message)
        {
            return null;
        }

        public void Shutdown()
        {
            _server.Shutdown("bye");
        }
    }
}
