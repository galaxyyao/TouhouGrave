using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring
{
    public partial class Game : Commands.ICause
    {
        public Player Winner
        {
            get;
            private set;
        }

        private void Main()
        {
            CurrentPhase = "Begin";
            Begin();
            //WaitForMessage("Start");

            InPlayerPhases = true;
            Round = 0;

            IssueCommandsAndFlush(new Commands.DrawCard(m_players[0]));

            for (; !AreWinnersDecided(); m_actingPlayer = ++m_actingPlayer % m_players.Length)
            {
                Round++;
                IsWarriorPlayedThisTurn = false;

                CurrentPhase = "PhaseA";
                IssueCommandsAndFlush(new Commands.StartTurn { });

                CurrentPhase = "Tactical";

                while (true)
                {
                    var result = new Interactions.TacticalPhase(ActingPlayer).Run();
                    if (result.ActionType == TacticalPhase.Action.PlayCard)
                    {
                        var cardToPlay = (BaseCard)result.Data;
                        Debug.Assert(cardToPlay.Owner == ActingPlayer);
                        IssueCommandsAndFlush(new Commands.PlayCard(cardToPlay));
                    }
                    else if (result.ActionType == TacticalPhase.Action.CastSpell)
                    {
                        var spellToCast = (Behaviors.ICastableSpell)result.Data;
                        Debug.Assert(spellToCast.Host.Owner == ActingPlayer);
                        IssueCommandsAndFlush(new Commands.CastSpell(spellToCast));
                    }
                    else if (result.ActionType == TacticalPhase.Action.DrawCard)
                    {
                        IssueCommandsAndFlush(
                            new Commands.UpdateMana(ActingPlayer, -1, this),
                            new Commands.DrawCard(ActingPlayer));
                    }
                    else if (result.ActionType == TacticalPhase.Action.Skip)
                    {
                        break;
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }

                CurrentPhase = "Combat/Attack";
                IssueCommandsAndFlush(new Commands.StartAttackPhase { });
                var declaredAttackers = new Interactions.SelectCards(
                    ActingPlayer,
                    ActingPlayer.CardsOnBattlefield.Where(card =>
                        card.Behaviors.Has<Behaviors.Warrior>()
                        && card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.StandingBy).ToArray().ToIndexable(),
                    Interactions.SelectCards.SelectMode.Multiple,
                    "Select warriors in battlefield to make them attackers.").Run().Clone();

                CurrentPhase = "Combat/Block";
                IssueCommandsAndFlush(new Commands.StartBlockPhase { });
                IIndexable<IIndexable<BaseCard>> declaredBlockers;
                while (true)
                {
                    var opponentPlayer = ActingPlayerEnemies.First(); // TODO: multiplayer game
                    var result = new Interactions.BlockPhase(opponentPlayer, declaredAttackers).Run();
                    if (result.ActionType == BlockPhase.Action.ConfirmBlock)
                    {
                        declaredBlockers = (result.Data as IIndexable<IIndexable<BaseCard>>).Clone(e => e.Clone());
                        break;
                    }
                    else if (result.ActionType == BlockPhase.Action.PlayCard)
                    {
                        var cardToPlay = (BaseCard)result.Data;
                        Debug.Assert(cardToPlay.Owner == opponentPlayer);
                        IssueCommandsAndFlush(new Commands.PlayCard(cardToPlay));
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }

                CurrentPhase = "Combat/Resolve";
                ResolveCombat(declaredAttackers, declaredBlockers);

                CurrentPhase = "PhaseB";
                IssueCommandsAndFlush(
                    new Commands.ResetAccumulatedDamage(),
                    new Commands.UpdateMana(ActingPlayer, ActingPlayer.ManaDelta, this),
                    new Commands.EndTurn());
            };

            //InPlayerPhases = false;

            new Interactions.NotifyOnly(ActingPlayer.Controller, String.Format(CultureInfo.CurrentCulture, "{0} 获得了胜利", Winner.Name));
            CurrentPhase = "End";
            End();

            CurrentPhase = "PostEnd";
        }

        private bool AreWinnersDecided()
        {
            if (Players.Any(player => player.Health <= 0))
            {
                Winner = ActingPlayerEnemies.First();
                return true;
            }
            if (Players.Any(player => player.m_library.Count() <= 0))
            {
                Winner = ActingPlayerEnemies.First();
                return true;
            }

            return false;
        }
    }
}
