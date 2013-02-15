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

        public void RunTurn()
        {
            if (m_gameFlowThread != null && System.Threading.Thread.CurrentThread != m_gameFlowThread
                || CurrentPhase != "")
            {
                throw new InvalidOperationException("Can't run turn.");
            }

            IssueCommandsAndFlush(new Commands.StartTurn(ActingPlayer));

            IssueCommand(new Commands.StartPhase("Upkeep"));
            // skip drawing card for the starting player in the first round
            if (Round > 1 || m_actingPlayer != 0)
            {
                IssueCommands(new Commands.DrawCard(ActingPlayer));
            }
            IssueCommands(new Commands.UpdateMana(ActingPlayer, ActingPlayer.MaxMana - ActingPlayer.Mana, this));
            ActingPlayer.CardsOnBattlefield
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                .ForEach(card => IssueCommands(
                    new Commands.SendBehaviorMessage(card.Behaviors.Get<Behaviors.Warrior>(), "GoStandingBy", null)));
            IssueCommandsAndFlush(new Commands.EndPhase());

            DidSacrifice = false;
            DidRedeem = false;
            IssueCommandsAndFlush(new Commands.StartPhase("Main"));

            RunTurnFromMainPhase();
        }

        public void RunTurnFromMainPhase()
        {
            RunTurnFromMainPhase(null);
        }

        public void RunTurnFromMainPhase(Interactions.TacticalPhase.CompiledResponse compiledMainPhaseResponse)
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
                if (result.ActionType == Interactions.TacticalPhase.Action.PlayCard)
                {
                    var cardToPlay = (BaseCard)result.Data;
                    Debug.Assert(cardToPlay.Owner == ActingPlayer);
                    IssueCommandsAndFlush(new Commands.PlayCard(cardToPlay));
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.ActivateAssist)
                {
                    var cardToActivate = (BaseCard)result.Data;
                    Debug.Assert(cardToActivate.Owner == ActingPlayer);
                    IssueCommandsAndFlush(new Commands.ActivateAssist(cardToActivate));
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.CastSpell)
                {
                    var spellToCast = (Behaviors.ICastableSpell)result.Data;
                    Debug.Assert(spellToCast.Host.Owner == ActingPlayer);
                    IssueCommandsAndFlush(new Commands.CastSpell(spellToCast));
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.Sacrifice)
                {
                    var cardToSacrifice = (BaseCard)result.Data;
                    IssueCommandsAndFlush(
                        new Commands.Sacrifice(cardToSacrifice),
                        new Commands.UpdateMana(ActingPlayer, 1, this));
                    DidSacrifice = true;
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.Redeem)
                {
                    var cardToRedeem = (BaseCard)result.Data;
                    IssueCommandsAndFlush(new Commands.Redeem(cardToRedeem));
                    DidRedeem = true;
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.AttackCard)
                {
                    var pair = (BaseCard[])result.Data;
                    var attackerWarrior = pair[0].Behaviors.Get<Behaviors.Warrior>();
                    IssueCommandsAndFlush(
                        new Commands.DealDamageToCard(
                            pair[1], attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null)
                        );
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.AttackPlayer)
                {
                    var pair = (object[])result.Data;
                    var attackerWarrior = (pair[0] as BaseCard).Behaviors.Get<Behaviors.Warrior>();
                    IssueCommandsAndFlush(
                        new Commands.DealDamageToPlayer(
                            pair[1] as Player, attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null)
                        );
                }
                else if (result.ActionType == Interactions.TacticalPhase.Action.Pass)
                {
                    break;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            IssueCommandsAndFlush(
                new Commands.EndPhase(),
                new Commands.StartPhase("Cleanup"),
                new Commands.EndPhase(),
                new Commands.EndTurn(ActingPlayer));
        }

        private void GameFlowMain()
        {
            IssueCommand(new Commands.StartPhase("Begin"));

            foreach (var player in Players)
            {
                // shuffle player's library
                IssueCommand(new Commands.ShuffleLibrary(player));

                // draw initial hands
                7.Repeat(i => IssueCommand(new Commands.DrawCard(player)));
            }

            IssueCommandsAndFlush(new Commands.EndPhase());

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
