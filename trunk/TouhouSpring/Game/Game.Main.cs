using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game : Commands.ICause
    {
        public Player Winner
        {
            get; private set;
        }

        public bool DidSacrifice
        {
            get; private set;
        }

        public bool DidRedeem
        {
            get; private set;
        }

        public bool RunTurn()
        {
            if (m_gameFlowThread != null && System.Threading.Thread.CurrentThread != m_gameFlowThread
                || CurrentPhase != "")
            {
                throw new InvalidOperationException("Can't run turn.");
            }

            QueueCommandsAndFlush(new Commands.StartTurn(ActingPlayer));

            QueueCommand(new Commands.StartPhase("Upkeep"));
            // skip drawing card for the starting player in the first round
            if (Round > 1 || m_actingPlayer != 0)
            {
                QueueCommand(new Commands.DrawCard(ActingPlayer));
            }
            QueueCommand(new Commands.AddPlayerMana(ActingPlayer, ActingPlayer.MaxMana - ActingPlayer.Mana, true, this));
            ActingPlayer.CardsOnBattlefield
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                .ForEach(card => QueueCommand(
                    new Commands.SendBehaviorMessage(card.Behaviors.Get<Behaviors.Warrior>(), "GoStandingBy", null)));
            QueueCommandsAndFlush(new Commands.EndPhase());

            DidSacrifice = false;
            DidRedeem = false;
            QueueCommandsAndFlush(new Commands.StartPhase("Main"));

            return RunTurnFromMainPhase();
        }

        public bool RunTurnFromMainPhase()
        {
            return RunTurnFromMainPhase(null);
        }

        public bool RunTurnFromMainPhase(Interactions.TacticalPhase.CompiledResponse compiledMainPhaseResponse)
        {
            if (m_gameFlowThread != null && System.Threading.Thread.CurrentThread != m_gameFlowThread
                || CurrentPhase != "Main")
            {
                throw new InvalidOperationException("Can't run turn from main phase.");
            }

            while (true)
            {
                var result = compiledMainPhaseResponse != null
                             ? compiledMainPhaseResponse.Restore(ActingPlayer)
                             : new Interactions.TacticalPhase(ActingPlayer).Run();
                compiledMainPhaseResponse = null;
                if (result.ActionType == Interactions.BaseInteraction.PlayerAction.PlayCard)
                {
                    var cardToPlay = (CardInstance)result.Data;
                    Debug.Assert(cardToPlay.Owner == ActingPlayer);
                    QueueCommandsAndFlush(new Commands.PlayCard(cardToPlay));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.ActivateAssist)
                {
                    var cardToActivate = (CardInstance)result.Data;
                    Debug.Assert(cardToActivate.Owner == ActingPlayer);
                    foreach (var card in ActingPlayer.ActivatedAssits)
                    {
                        QueueCommand(new Commands.DeactivateAssist(card));
                    }
                    QueueCommandsAndFlush(new Commands.ActivateAssist(cardToActivate));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.CastSpell)
                {
                    var spellToCast = (Behaviors.ICastableSpell)result.Data;
                    Debug.Assert(spellToCast.Host.Owner == ActingPlayer);
                    QueueCommandsAndFlush(new Commands.CastSpell(spellToCast));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Sacrifice)
                {
                    var cardToSacrifice = (CardInstance)result.Data;
                    QueueCommandsAndFlush(
                        new Commands.Sacrifice(cardToSacrifice),
                        new Commands.AddPlayerMana(ActingPlayer, 1, true, this));
                    DidSacrifice = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Redeem)
                {
                    var cardToRedeem = (CardInstance)result.Data;
                    QueueCommandsAndFlush(new Commands.Redeem(cardToRedeem));
                    DidRedeem = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackCard)
                {
                    var pair = (CardInstance[])result.Data;
                    var attackerWarrior = pair[0].Behaviors.Get<Behaviors.Warrior>();
                    QueueCommandsAndFlush(
                        new Commands.DealDamageToCard(
                            pair[1], attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null)
                        );
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackPlayer)
                {
                    var pair = (object[])result.Data;
                    var attackerWarrior = (pair[0] as CardInstance).Behaviors.Get<Behaviors.Warrior>();
                    QueueCommandsAndFlush(
                        new Commands.SubtractPlayerLife(
                            pair[1] as Player, attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null)
                        );
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Pass)
                {
                    break;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Abort)
                {
                    return false;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            QueueCommandsAndFlush(
                new Commands.EndPhase(),
                new Commands.StartPhase("Cleanup"),
                new Commands.EndPhase(),
                new Commands.EndTurn(ActingPlayer));

            return true;
        }

        private void GameFlowMain()
        {
            QueueCommand(new Commands.StartPhase("Begin"));

            foreach (var player in Players)
            {
                // shuffle player's library
                QueueCommand(new Commands.ShuffleLibrary(player));

                // draw initial hands
                7.Repeat(i => QueueCommand(new Commands.DrawCard(player)));
            }

            QueueCommandsAndFlush(new Commands.EndPhase());

            // TODO: Non-trivial determination of the acting player for the first turn
            m_actingPlayer = 0;
            Round = 0;

            for (; !AreWinnersDecided(); m_actingPlayer = ++m_actingPlayer % m_players.Length)
            {
                Round++;
                RunTurn();
            };

            //InPlayerPhases = false;

            //new Interactions.NotifyOnly(ActingPlayer.Controller, String.Format(CultureInfo.CurrentCulture, "{0} 获得了胜利", Winner.Name));
        }

        private bool AreWinnersDecided()
        {
            if (Players.Any(player => player.Health <= 0))
            {
                Winner = ActingPlayerEnemies.First();
                return true;
            }
            if (Players.Any(player => player.Library.Count <= 0))
            {
                Winner = ActingPlayerEnemies.First();
                return true;
            }

            return false;
        }
    }
}
