using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        public int RoomId
        {
            get;
            private set;
        }

        public enum RoomStatusEnum
        {
            Waiting, Starting, Started, End
        }

        public RoomStatusEnum RoomStatus
        {
            get;
            set;
        }

        public int Seed
        {
            get;
            private set;
        }

        public int StartupIndex
        {
            get;
            private set;
        }
    }
}
