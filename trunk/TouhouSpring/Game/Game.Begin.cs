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
                IssueCommand(new Commands.ShuffleLibrary { PlayerShuffling = player });

                // draw initial hands
                5.Repeat(i => IssueCommand(new Commands.DrawCard { PlayerDrawing = player }));
            }

            FlushCommandQueue();

            // TODO: Non-trivial determination of the acting player for the first turn
            m_actingPlayer = 0;
        }
    }
}
