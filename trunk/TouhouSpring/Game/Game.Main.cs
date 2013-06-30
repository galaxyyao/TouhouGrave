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

            StackAndFlush(new Commands.StartTurn(ActingPlayer));

            var ctx = CreateResolveContext();
            ctx.QueueCommand(new Commands.StartPhase("Upkeep"));
            // skip drawing card for the starting player in the first round
            if (Round > 1 || m_actingPlayer != 0)
            {
                ctx.QueueCommand(new Commands.DrawMove(ActingPlayer, SystemZone.Hand));
            }
            ctx.QueueCommand(new Commands.AddPlayerMana(ActingPlayer, ActingPlayer.MaxMana - ActingPlayer.Mana, true, this));
            ActingPlayer.CardsOnBattlefield
                .Where(card => card.Warrior != null)
                .ForEach(card => ctx.QueueCommand(
                    new Commands.SendBehaviorMessage(card.Warrior, "GoStandingBy", null)));
            ctx.QueueCommand(new Commands.EndPhase());
            StackAndFlush(ctx);

            DidSacrifice = false;
            DidRedeem = false;
            StackAndFlush(new Commands.StartPhase("Main"));

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
                    StackAndFlush(new Commands.PlayCard(cardToPlay, SystemZone.Battlefield, this));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.ActivateAssist)
                {
                    var cardToActivate = (CardInstance)result.Data;
                    Debug.Assert(cardToActivate.Owner == ActingPlayer);
                    var ctx = CreateResolveContext();
                    foreach (var card in ActingPlayer.ActivatedAssits)
                    {
                        ctx.QueueCommand(new Commands.DeactivateAssist(card));
                    }
                    ctx.QueueCommand(new Commands.ActivateAssist(cardToActivate));
                    StackAndFlush(ctx);
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.CastSpell)
                {
                    var spellToCast = (Behaviors.ICastableSpell)result.Data;
                    Debug.Assert(spellToCast.Host.Owner == ActingPlayer);
                    StackAndFlush(new Commands.CastSpell(spellToCast));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Sacrifice)
                {
                    var cardToSacrifice = (CardInstance)result.Data;
                    StackAndFlush(
                        new Commands.InitiativeMoveCard(cardToSacrifice, SystemZone.Sacrifice, this),
                        new Commands.AddPlayerMana(ActingPlayer, 1, true, this));
                    DidSacrifice = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Redeem)
                {
                    var cardToRedeem = (CardInstance)result.Data;
                    StackAndFlush(new Commands.InitiativeMoveCard(cardToRedeem, SystemZone.Hand, this));
                    DidRedeem = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackCard)
                {
                    var pair = (CardInstance[])result.Data;
                    var attackerWarrior = pair[0].Warrior;
                    StackAndFlush(
                        new Commands.DealDamageToCard(pair[1], attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(attackerWarrior, "GoCoolingDown", null));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackPlayer)
                {
                    var pair = (object[])result.Data;
                    var attackerWarrior = (pair[0] as CardInstance).Warrior;
                    StackAndFlush(
                        new Commands.SubtractPlayerLife(pair[1] as Player, attackerWarrior.Attack, attackerWarrior),
                        new Commands.SendBehaviorMessage(attackerWarrior, "GoCoolingDown", null));
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

            StackAndFlush(
                new Commands.EndPhase(),
                new Commands.StartPhase("Cleanup"),
                new Commands.EndPhase(),
                new Commands.EndTurn(ActingPlayer));

            return true;
        }

        private void GameFlowMain()
        {
            var ctx = CreateResolveContext();
            ctx.QueueCommand(new Commands.StartPhase("Begin"));

            foreach (var player in Players)
            {
                // shuffle player's library
                ctx.QueueCommand(new Commands.ShuffleLibrary(player));

                // draw initial hands
                7.Repeat(i => ctx.QueueCommand(new Commands.DrawMove(player, SystemZone.Hand, this)));
            }

            ctx.QueueCommand(new Commands.EndPhase());
            StackAndFlush(ctx);

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
