﻿using System;
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
                                #region user connect
                                //Ask user to wait for another player, if there's no idle room
                                int enteredRoomId = UserEnter(im.SenderConnection);
                                SendMessage(im.SenderConnection, string.Format("{0} enterroom", enteredRoomId));
                                if (_roomList[enteredRoomId].Status == Room.RoomStatus.Idle)
                                {
                                    SendMessage(im.SenderConnection, string.Format("{0} waiting", enteredRoomId));
                                }
                                else
                                {
                                    int seed = gameStartSeed.Next();
                                    SendMessage(enteredRoomId, string.Format("{0} generateseed {1}", enteredRoomId, seed));
                                    foreach (var playerConn in _roomList[enteredRoomId].PlayerConns)
                                    {
                                        SendMessage(playerConn
                                            , string.Format("{0} startgame {1}", enteredRoomId
                                            , _roomList[enteredRoomId].GetPlayerIndex(playerConn.RemoteUniqueIdentifier) - seed % 2));//Random who start the 1st turn
                                    }
                                }
                                #endregion
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                #region user disconnect
                                long disconnectedPlayerUid = im.SenderConnection.RemoteUniqueIdentifier;
                                int disconnectedRoomId = GetRoomIdByUid(disconnectedPlayerUid);
                                if (_roomList.ContainsKey(disconnectedRoomId))
                                {
                                    SendMessage(_roomList[disconnectedRoomId].GetOpponentPlayerConnection(disconnectedPlayerUid)
                                        , string.Format("{0} disconnect", disconnectedRoomId));
                                    _roomList[disconnectedRoomId] = null;
                                    _roomList.Remove(disconnectedRoomId);
                                }
                                #endregion user disconnect
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Console.WriteLine(text);
                            InterpretMessage(im.SenderConnection, text);

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

        /// <summary>
        /// Send message to single player according to player's NetConnection
        /// </summary>
        /// <param name="playerConn"></param>
        /// <param name="message"></param>
        private void SendMessage(NetConnection playerConn, string message)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.Write(message);
            _server.SendMessage(om, playerConn, NetDeliveryMethod.ReliableOrdered, 0);
        }

        /// <summary>
        /// Send same message to all players in 1 room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="message"></param>
        private void SendMessage(int roomId, string message)
        {
            foreach (var playerConn in _roomList[roomId].PlayerConns)
            {
                NetOutgoingMessage om = _server.CreateMessage();
                om.Write(message);
                _server.SendMessage(om, playerConn, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        /// <summary>
        /// Broadcast message to all users on the server
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(string message)
        {
            foreach (KeyValuePair<int, Room> roomPair in _roomList)
            {
                foreach (var playerConn in roomPair.Value.PlayerConns)
                {
                    NetOutgoingMessage om = _server.CreateMessage();
                    om.Write(message);
                    _server.SendMessage(om, playerConn, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
        }

        private void InterpretMessage(NetConnection senderConn, string message)
        {
            List<string> parts = message.Split(' ').ToList();
            switch (parts[1])
            {
                case "roomentered":
                case "gamestarted":
                    break;
                case "switchturn":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} switchturn", roomId));
                    }
                    break;
                case "sacrifice":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} sacrifice {1}", roomId, parts[2]));
                    }
                    break;
                case "playcard":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} playcard {1}", roomId, parts[2]));
                    }
                    break;
                case "attackcard":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} attackcard {1} {2}", roomId, parts[2], parts[3]));
                    }
                    break;
                case "attackplayer":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} attackplayer {1}", roomId, parts[2]));
                    }
                    break;
                case "activateassist":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} activateassist {1}", roomId, parts[2]));
                    }
                    break;
                case "redeem":
                    {
                        int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
                        SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier)
                            , string.Format("{0} redeem {1}", roomId, parts[2]));
                    }
                    break;
                default:
                    break;
            }
        }

        public void Shutdown()
        {
            _server.Shutdown("0 servershutdown");
        }
    }
}
