using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent
    {
        private double Evaluate(Game game, int allyPlayerIndex)
        {
            double sum = 0;
            for (int i = 0; i < game.Players.Count; ++i)
            {
                sum += (i == allyPlayerIndex ? 1 : -1) * Evaluate(game.Players[i]);
            }
            return sum;
        }

        private double Evaluate(Player player)
        {
            const double PlayerLifeCurvePower = 0.33333;
            //const double PlayerManaCurvePower = 0.33333;
            const double HandCountCurvePower = 0.5;
            const double CardAttackCurvePower = 0.8;
            //const double LifeCurvePower = 

            const double PlayerLifeWeight = 10;
            const double PlayerManaWeight = 10;
            const double HandCountWeight = 0;
            const double CardAttackWeight = 5;

            // player life
            double playerLifeScore = player.Health <= 0 ? -10000 : Math.Pow(player.Health, PlayerLifeCurvePower);
            
            // mana pool size
            double manaPoolScore = Math.Log(player.MaxMana + 1);

            // hand count
            double handCountScore = Math.Pow(player.CardsOnHand.Count, HandCountCurvePower);

            // total card attacks
            int totalCardAttacks = player.CardsOnBattlefield.Sum(card =>
                card.Behaviors.Has<Behaviors.Warrior>() ? card.Behaviors.Get<Behaviors.Warrior>().Attack : 0);
            double cardAttackScore = Math.Pow(totalCardAttacks, CardAttackCurvePower);

            // TODO: sum of each card lifes

            return playerLifeScore * PlayerLifeWeight
                   + manaPoolScore * PlayerManaWeight
                   + handCountScore * HandCountWeight
                   + cardAttackScore * CardAttackWeight;
        }
    }
}
