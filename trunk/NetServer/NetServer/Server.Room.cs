using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace TouhouSpring.NetServerCore
{
    public partial class Server
    {
        private Dictionary<int, Room> _roomList = new Dictionary<int, Room>();

        private int _idleRoomId = -1;

        public int UserEnter(NetConnection playerConn)
        {
            if (_idleRoomId == -1)
            {
                int createdRoomId = Find1stEmptyRoomNum();
                Room createdRoom = new Room(createdRoomId, playerConn);
                _roomList.Add(createdRoomId, createdRoom);
                _idleRoomId = createdRoomId;
                return -1;
            }
            _roomList[_idleRoomId].Player2Enter(playerConn);
            int enteredRoomId = _idleRoomId;
            _idleRoomId = -1;
            return enteredRoomId;
        }

        private int Find1stEmptyRoomNum()
        {
            int i = 1;
            while (_roomList.ContainsKey(i))
                i++;
            return i;
        }

        enum RoomStatus
        {
            Idle, Playing
        }

        private class Room
        {
            public RoomStatus Status
            {
                get;
                private set;
            }

            public int Id
            {
                get;
                private set;
            }

            public long Player1Uid
            {
                get;
                private set;
            }

            public long Player2Uid
            {
                get;
                private set;
            }

            public NetConnection Player1Connection
            {
                get;
                private set;
            }

            public NetConnection Player2Connection
            {
                get;
                private set;
            }

            public Room(int id, NetConnection player1Conn)
            {
                Status = RoomStatus.Idle;
                Id = id;
                Player1Uid = player1Conn.RemoteUniqueIdentifier;
                Player1Connection = player1Conn;
                Player2Uid = -1;
                Player2Connection = null;
            }

            public void Player2Enter(NetConnection player2Conn)
            {
                Player2Uid = player2Conn.RemoteUniqueIdentifier;
                Player2Connection = player2Conn;
            }
        }
    }
}
