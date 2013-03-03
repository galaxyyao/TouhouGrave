using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {


        public static class CurrentCommand
        {
            public enum InteractionType
            {
                TacticalPhase, SelectCards, Others
            }

            public static InteractionType Type
            {
                get;
                set;
            }

            public static object Result
            {
                get;
                set;
            }

            public static int ResultSubjectIndex
            {
                get;
                set;
            }

            public static int[] ResultParameters
            {
                get;
                set;
            }
        }

    }
}
