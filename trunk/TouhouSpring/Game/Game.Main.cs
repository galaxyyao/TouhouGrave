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

            for (; !AreWinnersDecided(); m_actingPlayer = ++m_actingPlayer % m_players.Length)
            {
                Round++;

                CurrentPhase = "Upkeep";
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
                var declaredAttackers = new Interactions.DeclareAttackers(ActingPlayer).Run().Clone();

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
