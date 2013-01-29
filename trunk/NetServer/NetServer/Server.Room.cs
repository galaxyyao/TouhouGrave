using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.NetServerCore
{
    public partial class Server
    {
        private Dictionary<int, Room> _roomList = new Dictionary<int, Room>();

        private int _idleRoomId = -1;

        public int UserEnter(long uid)
        {
            if (_idleRoomId == -1)
            {
                int createdRoomId = Find1stEmptyRoomNum();
                Room createdRoom = new Room(createdRoomId, uid);
                _roomList.Add(createdRoomId, createdRoom);
                _idleRoomId = createdRoomId;
                return -1;
            }
            _roomList[_idleRoomId].Enter(uid);
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

            public Room(int id, long player1Uid)
            {
                Status = RoomStatus.Idle;
                Id = id;
                Player1Uid = player1Uid;
                Player2Uid = -1;
            }

            public void Enter(long player2Uid)
            {
                Player2Uid = player2Uid;
            }
        }
    }
}
