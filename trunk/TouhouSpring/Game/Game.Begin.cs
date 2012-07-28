using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        /// <summary>
        /// Call to actually begin a game.
        /// </summary>
        private void Begin()
        {
            foreach (var player in Players)
            {
                // shuffle player's library
                RunCommand(new Commands.ShuffleLibrary { PlayerShuffling = player });

                // draw initial hands
                5.Repeat(i => RunCommand(new Commands.DrawCard { PlayerDrawing = player }));
            }

            // TODO: Non-trivial determination of the acting player for the first turn
            m_actingPlayer = 0;
        }
    }
}
