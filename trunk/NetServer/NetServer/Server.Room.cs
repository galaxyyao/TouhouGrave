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

        internal int UserEnter(NetConnection playerConn)
        {
            if (_idleRoomId == -1)
            {
                int createdRoomId = Find1stEmptyRoomNum();
                Room createdRoom = new Room(createdRoomId);
                _roomList.Add(createdRoomId, createdRoom);
                _idleRoomId = createdRoomId;
                createdRoom.PlayerEnter(playerConn);
                return createdRoomId;
            }

            _roomList[_idleRoomId].PlayerEnter(playerConn);
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

        private int GetRoomIdByUid(long playerUid)
        {
            var playerRoomId = _roomList.FirstOrDefault(
                room => (room.Value.PlayerUids.Contains(playerUid))
                ).Key;
            return playerRoomId;
        }
    }
}
