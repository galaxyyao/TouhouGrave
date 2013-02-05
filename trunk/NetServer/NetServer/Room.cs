using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace TouhouSpring.NetServerCore
{
    public class Room
    {
        public enum RoomStatus
        {
            Idle, Full
        }

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

        public int CurrentPlayerNumber
        {
            get;
            private set;
        }

        public List<long> PlayerUids;

        public List<NetConnection> PlayerConns;

        public NetConnection GetPlayerConnection(long playerUid)
        {
            int index = PlayerUids.IndexOf(playerUid);
            return PlayerConns[index];
        }

        //TODO: Change for future 3 or more players
        public NetConnection GetOpponentPlayerConnection(long playerUid)
        {
            int index = PlayerUids.IndexOf(playerUid);
            return (index == 0) ? PlayerConns[1] : PlayerConns[0];
        }

        public Room(int id)
        {
            Status = RoomStatus.Idle;
            Id = id;
            PlayerUids = new List<long>();
            PlayerConns = new List<NetConnection>();
            CurrentPlayerNumber = 0;
        }

        public void PlayerEnter(NetConnection playerConn)
        {
            PlayerUids.Add(playerConn.RemoteUniqueIdentifier);
            PlayerConns.Add(playerConn);
            CurrentPlayerNumber++;

            //TODO: Change for future 3 or more players
            if (CurrentPlayerNumber == 2)
            {
                Status = RoomStatus.Full;
            }
            if (CurrentPlayerNumber > 2)
            {
                throw new Exception("Room can only afford 2 players at most!");
            }
        }

        public int GetPlayerIndex(long playerUid)
        {
            return PlayerUids.IndexOf(playerUid);
        }
    }
}
