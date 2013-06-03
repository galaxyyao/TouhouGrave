using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class ResolveContext
    {
        public Game Game
        {
            get; private set;
        }

        internal bool Abort
        {
            get; set;
        }

        internal ResolveContext(Game game)
        {
            Game = game;
        }
    }
}
