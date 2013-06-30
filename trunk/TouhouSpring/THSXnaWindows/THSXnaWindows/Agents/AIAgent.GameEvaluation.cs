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

        private double PrintEvaluate(Player player)
        {
            const double PlayerLifeCurvePower = 0.33333;
            const double HandCountCurvePower = 0.22;
            const double CardAttackCurvePower = 0.8;
            const double CardValueCurvePower = 0.8;

            const double PlayerLifeWeight = 10;
            const double PlayerManaWeight = 10;
            const double HandCountWeight = 0;
            const double CardAttackWeight = 4;
            const double CardValueWeight = 3;

            // player life
            double playerLifeScore = player.Health <= 0 ? -10000 : Math.Pow(player.Health, PlayerLifeCurvePower);
            System.Diagnostics.Debug.WriteLine(String.Format("\tPlayerLife: {0:0.00} x {1:0.00}", playerLifeScore, PlayerLifeWeight));

            // mana pool size
            double manaPoolScore = Math.Log(player.MaxMana + 1);
            System.Diagnostics.Debug.WriteLine(String.Format("\tManaPool: {0:0.00} x {1:0.00}", manaPoolScore, PlayerManaWeight));

            // hand count
            double handCountScore = Math.Pow(player.CardsOnHand.Count, HandCountCurvePower);

            // total card attacks
            double totalCardAttacks = 0;
            // total card values weighted by remaining life
            double totalCardValues = 0;
            foreach (var card in player.CardsOnBattlefield)
            {
                if (card.Warrior == null)
                {
                    continue;
                }

                totalCardAttacks += !card.Behaviors.Has<Behaviors.Unattackable>() ? card.Warrior.Attack : 0;

                double weight = Math.Max(1 - (double)card.Warrior.Life / card.Warrior.MaxLife, 0);
                weight = Math.Sqrt(1 - weight * weight);

                totalCardValues += GetScore(card.Model) * weight;
            }
            double cardAttackScore = Math.Pow(totalCardAttacks, CardAttackCurvePower);
            System.Diagnostics.Debug.WriteLine(String.Format("\tCardAttack: {0:0.00} x {1:0.00}", cardAttackScore, CardAttackWeight));

            double cardValueScore = Math.Pow(totalCardValues, CardValueCurvePower);
            System.Diagnostics.Debug.WriteLine(String.Format("\tCardValue: {0:0.00} x {1:0.00}", cardValueScore, CardValueWeight));

            // TODO: sum of each card lifes

            return playerLifeScore * PlayerLifeWeight
                   + manaPoolScore * PlayerManaWeight
                   + handCountScore * HandCountWeight
                   + cardAttackScore * CardAttackWeight
                   + cardValueScore * CardValueWeight;
        }

        private double Evaluate(Player player)
        {
            const double PlayerLifeCurvePower = 0.33333;
            const double HandCountCurvePower = 0.22;
            const double CardAttackCurvePower = 0.8;
            const double CardValueCurvePower = 0.8;

            const double PlayerLifeWeight = 10;
            const double PlayerManaWeight = 10;
            const double HandCountWeight = 0;
            const double CardAttackWeight = 4;
            const double CardValueWeight = 3;

            // player life
            double playerLifeScore = player.Health <= 0 ? -10000 : Math.Pow(player.Health, PlayerLifeCurvePower);
            
            // mana pool size
            double manaPoolScore = Math.Log(player.MaxMana + 1);

            // hand count
            double handCountScore = Math.Pow(player.CardsOnHand.Count, HandCountCurvePower);

            // total card attacks
            double totalCardAttacks = 0;
            // total card values weighted by remaining life
            double totalCardValues = 0;
            foreach (var card in player.CardsOnBattlefield)
            {
                if (card.Warrior == null)
                {
                    continue;
                }

                totalCardAttacks += !card.Behaviors.Has<Behaviors.Unattackable>() ? card.Warrior.Attack : 0;

                double weight = Math.Max(1 - (double)card.Warrior.Life / card.Warrior.MaxLife, 0);
                weight = Math.Sqrt(1 - weight * weight);

                totalCardValues += GetScore(card.Model) * weight;
            }
            double cardAttackScore = Math.Pow(totalCardAttacks, CardAttackCurvePower);
            double cardValueScore = Math.Pow(totalCardValues, CardValueCurvePower);

            return playerLifeScore * PlayerLifeWeight
                   + manaPoolScore * PlayerManaWeight
                   + handCountScore * HandCountWeight
                   + cardAttackScore * CardAttackWeight
                   + cardValueScore * CardValueWeight;
        }
    }
}
