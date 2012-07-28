using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Shuffle a player's library
    /// </summary>
    public class ShuffleLibrary : ICommand
    {
        public string Token
        {
            get { return "ShuffleLibrary"; }
        }

        /// <summary>
        /// The player whose library is going to be shuffled
        /// </summary>
        public Player PlayerShuffling
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (PlayerShuffling == null)
            {
                throw new CommandValidationFailException("PlayerShuffling can't be null.");
            }
            else if (!game.Players.Contains(PlayerShuffling))
            {
                throw new CommandValidationFailException("The Player object is not registered in game.");
            }
        }

        public void RunMain(Game game)
        {
            PlayerShuffling.m_library.Shuffle(game.Random);
        }
    }
}
