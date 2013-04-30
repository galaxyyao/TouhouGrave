using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    internal partial class ResolveContext
    {
        public Game Game
        {
            get; private set;
        }

        public ResolveContext(Game game)
        {
            Game = game;
        }
    }
}
