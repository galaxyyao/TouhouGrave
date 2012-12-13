﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        public void ReserveCard(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }
            else if (RunningCommand == null)
            {
                throw new InvalidOperationException("Resource can only be reserved during command's execution phases.");
            }
            else if (!Players.Any(p => p.CardsOnBattlefield.Contains(card)))
            {
                throw new InvalidOperationException("Only cards on the battlefield can be reserved.");
            }

            // TODO: reserve the card
            throw new NotImplementedException();
        }

        public void ReserveHealth(Player player, int amount)
        {
            if (RunningCommand == null
                || RunningCommand.ExecutionPhase != Commands.CommandPhase.Prerequisite && RunningCommand.ExecutionPhase != Commands.CommandPhase.Setup)
            {
                throw new InvalidOperationException("Health can only be reserved at command's prerequisite or setup phase.");
            }
            else if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Argument amount must be greater than zero.");
            }
            else if (!Players.Contains(player))
            {
                throw new ArgumentException("Invalid player.");
            }
            else if (player.FreeHealth < amount)
            {
                throw new InvalidOperationException("Insufficient free health to be reserved.");
            }

            player.ReservedHealth += amount;
        }

        public void ReserveMana(Player player, int amount)
        {
            if (RunningCommand == null
                || RunningCommand.ExecutionPhase != Commands.CommandPhase.Prerequisite && RunningCommand.ExecutionPhase != Commands.CommandPhase.Setup)
            {
                throw new InvalidOperationException("Mana can only be reserved at command's prerequisite or setup phase.");
            }
            else if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Argument amount must be greater than zero.");
            }
            else if (!Players.Contains(player))
            {
                throw new ArgumentException("Invalid player.");
            }
            else if (player.FreeMana < amount)
            {
                throw new InvalidOperationException("Insufficient free mana to be reserved.");
            }

            player.ReservedMana += amount;
        }

        internal void ClearReservations()
        {
            Players.ForEach(player =>
            {
                player.ReservedHealth = 0;
                player.ReservedMana = 0;
            });
        }
    }
}
