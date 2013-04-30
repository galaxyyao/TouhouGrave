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

            m_currentResolveContext = new ResolveContext(this);
            m_currentResolveContext.QueueCommandsAndFlush(new Commands.StartTurn(ActingPlayer));
            m_currentResolveContext = null;

            m_currentResolveContext = new ResolveContext(this);
            m_currentResolveContext.QueueCommand(new Commands.StartPhase("Upkeep"));
            // skip drawing card for the starting player in the first round
            if (Round > 1 || m_actingPlayer != 0)
            {
                m_currentResolveContext.QueueCommand(new Commands.DrawCard(ActingPlayer));
            }
            m_currentResolveContext.QueueCommand(new Commands.AddPlayerMana(ActingPlayer, ActingPlayer.MaxMana - ActingPlayer.Mana, true, this));
            ActingPlayer.CardsOnBattlefield
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                .ForEach(card => m_currentResolveContext.QueueCommand(
                    new Commands.SendBehaviorMessage(card.Behaviors.Get<Behaviors.Warrior>(), "GoStandingBy", null)));
            m_currentResolveContext.QueueCommandsAndFlush(new Commands.EndPhase());
            m_currentResolveContext = null;

            DidSacrifice = false;
            DidRedeem = false;
            m_currentResolveContext = new ResolveContext(this);
            m_currentResolveContext.QueueCommandsAndFlush(new Commands.StartPhase("Main"));
            m_currentResolveContext = null;

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
                List<Commands.BaseCommand> commands = new List<Commands.BaseCommand>();
                if (result.ActionType == Interactions.BaseInteraction.PlayerAction.PlayCard)
                {
                    var cardToPlay = (CardInstance)result.Data;
                    Debug.Assert(cardToPlay.Owner == ActingPlayer);
                    commands.Add(new Commands.PlayCard(cardToPlay));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.ActivateAssist)
                {
                    var cardToActivate = (CardInstance)result.Data;
                    Debug.Assert(cardToActivate.Owner == ActingPlayer);
                    foreach (var card in ActingPlayer.ActivatedAssits)
                    {
                        commands.Add(new Commands.DeactivateAssist(card));
                    }
                    commands.Add(new Commands.ActivateAssist(cardToActivate));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.CastSpell)
                {
                    var spellToCast = (Behaviors.ICastableSpell)result.Data;
                    Debug.Assert(spellToCast.Host.Owner == ActingPlayer);
                    commands.Add(new Commands.CastSpell(spellToCast));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Sacrifice)
                {
                    var cardToSacrifice = (CardInstance)result.Data;
                    commands.Add(new Commands.Sacrifice(cardToSacrifice));
                    commands.Add(new Commands.AddPlayerMana(ActingPlayer, 1, true, this));
                    DidSacrifice = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.Redeem)
                {
                    var cardToRedeem = (CardInstance)result.Data;
                    commands.Add(new Commands.Redeem(cardToRedeem));
                    DidRedeem = true;
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackCard)
                {
                    var pair = (CardInstance[])result.Data;
                    var attackerWarrior = pair[0].Behaviors.Get<Behaviors.Warrior>();
                    commands.Add(new Commands.DealDamageToCard(
                            pair[1], attackerWarrior.Attack, attackerWarrior));
                    commands.Add(new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null));
                }
                else if (result.ActionType == Interactions.BaseInteraction.PlayerAction.AttackPlayer)
                {
                    var pair = (object[])result.Data;
                    var attackerWarrior = (pair[0] as CardInstance).Behaviors.Get<Behaviors.Warrior>();
                    commands.Add(new Commands.SubtractPlayerLife(
                            pair[1] as Player, attackerWarrior.Attack, attackerWarrior));
                    commands.Add(new Commands.SendBehaviorMessage(
                            attackerWarrior, "GoCoolingDown", null));
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

                if (commands.Count != 0)
                {
                    m_currentResolveContext = new ResolveContext(this);
                    m_currentResolveContext.QueueCommandsAndFlush(commands.ToArray());
                    m_currentResolveContext = null;
                }
            }

            m_currentResolveContext = new ResolveContext(this);
            m_currentResolveContext.QueueCommandsAndFlush(
                new Commands.EndPhase(),
                new Commands.StartPhase("Cleanup"),
                new Commands.EndPhase(),
                new Commands.EndTurn(ActingPlayer));
            m_currentResolveContext = null;

            return true;
        }

        private void GameFlowMain()
        {
            var ctx = new ResolveContext(this);
            ctx.QueueCommand(new Commands.StartPhase("Begin"));

            foreach (var player in Players)
            {
                // shuffle player's library
                ctx.QueueCommand(new Commands.ShuffleLibrary(player));

                // draw initial hands
                7.Repeat(i => ctx.QueueCommand(new Commands.DrawCard(player)));
            }

            ctx.QueueCommandsAndFlush(new Commands.EndPhase());

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
