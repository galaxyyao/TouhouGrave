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
            while (true)
            {

                _im = _server.ReadMessage();
                if (_im == null)
                    continue;
                switch (_im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = _im.ReadString();
                        Console.WriteLine(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)_im.ReadByte();
                            string reason = _im.ReadString();
                            Console.WriteLine(string.Format("{0} - {1}:{2}", NetUtility.ToHexString(_im.SenderConnection.RemoteUniqueIdentifier), status, reason));

                            if (status == NetConnectionStatus.Connected)
                            {
                                NetOutgoingMessage om = _server.CreateMessage();
                                om.Write(string.Format(" enterroom"));
                                _server.SendMessage(om, _im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //TODO:
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string data = _im.ReadString();
                            Console.WriteLine(data);
                            //TODO: Do something to Data
                            NetOutgoingMessage om = _server.CreateMessage();
                            om.Write(NetUtility.ToHexString(_im.SenderConnection.RemoteUniqueIdentifier) + " said: " + "aaa");
                            _server.SendMessage(om, _im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        break;
                    default:
                        Console.WriteLine(string.Format("Unhandled type: {0} {1} bytes {2}|{3}"
                            , _im.MessageType
                            , _im.LengthBytes
                            , _im.DeliveryMethod
                            , _im.SequenceChannel
                            ));
                        break;
                }
            }
        }

        public void Shutdown()
        {
            _server.Shutdown("bye");
        }
    }
}
