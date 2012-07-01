using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring
{
    public partial class Game
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

            //Instantiate HeroCard
            var hero = new BaseCard(PlayerPlayer.Hero.Host.Model, PlayerPlayer);
            PlayCard(hero);
            m_actingPlayer = ++m_actingPlayer % m_players.Length;
            hero = new BaseCard(PlayerPlayer.Hero.Host.Model, PlayerPlayer);
            PlayCard(hero);
            m_actingPlayer = ++m_actingPlayer % m_players.Length;

            Round = 0;

            for (; !AreWinnersDecided(); m_actingPlayer = ++m_actingPlayer % m_players.Length)
            {
                Round++;
                IsWarriorPlayedThisTurn = false;

                CurrentPhase = "PhaseA";
                ResetCardsStateInBattlefield(PlayerPlayer);
                TriggerGlobal(new Triggers.PlayerTurnStartedContext(this));

                CurrentPhase = "Tactical";

                while (true)
                {
                    var result = new Interactions.TacticalPhase(PlayerController).Run();
                    if (result.ActionType == TacticalPhase.Action.PlayCard)
                    {
                        var cardToPlay = (BaseCard)result.Data;
                        Debug.Assert(cardToPlay.Owner == PlayerPlayer);
                        PlayCard(cardToPlay);
                    }
                    else if (result.ActionType == TacticalPhase.Action.CastSpell)
                    {
                        var spellToCast = (Behaviors.ICastableSpell)result.Data;
                        Debug.Assert(spellToCast.Host.Owner == PlayerPlayer);
                        CastSpell(spellToCast);
                    }
                    else if (result.ActionType == TacticalPhase.Action.DrawCard)
                    {
                        if (UpdateMana(PlayerPlayer, -1))
                        {
                            DrawCard(PlayerPlayer);
                        }
                    }
                    else if (result.ActionType == TacticalPhase.Action.Skip)
                    {
                        break;
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                    ResolveBattlefieldCards();
                }

                CurrentPhase = "Combat/Attack";
                TriggerGlobal(new Triggers.AttackPhaseStartedContext(this));
                PlayerPlayer.Hero.Host.State = CardState.CoolingDown;
                var declaredAttackers = new Interactions.SelectCards(
                    PlayerController,
                    PlayerPlayer.CardsOnBattlefield.Where(card => card.Behaviors.Has<Behaviors.Warrior>() && card.State == CardState.StandingBy).ToArray().ToIndexable(),
                    Interactions.SelectCards.SelectMode.Multiple,
                    "Select warriors in battlefield to make them attackers.").Run().Clone();
                TriggerGlobal(new Triggers.AttackPhaseEndedContext(this));

                CurrentPhase = "Combat/Block";
                TriggerGlobal(new Triggers.BlockPhaseStartedContext(this, declaredAttackers));
                IIndexable<IIndexable<BaseCard>> declaredBlockers;
                while (true)
                {
                    object selected = new Interactions.BlockPhase(OpponentController, declaredAttackers).Run();

                    if (selected is BaseCard)
                    {
                        var cardToPlay = (BaseCard)selected;
                        Debug.Assert(cardToPlay.Owner == OpponentPlayer);
                        PlayCard(cardToPlay);
                    }
                    else if (selected is IIndexable<IIndexable<BaseCard>>)
                    {
                        declaredBlockers = ((IIndexable<IIndexable<BaseCard>>)selected).Clone(e => e.Clone());
                        break;
                    }
                    ResolveBattlefieldCards();
                }
                TriggerGlobal(new Triggers.BlockPhaseEndedContext(this));

                CurrentPhase = "Combat/Resolve";
                ResolveCombat(declaredAttackers, declaredBlockers);
                ResolveBattlefieldCards();
                ResetAccumulatedDamage();

                TriggerGlobal(new Triggers.PlayerTurnEndedContext(this));

                UpdateMana(PlayerPlayer, PlayerPlayer.ManaDelta);
                PlayerPlayer.ResetManaDelta();
            };

            //InPlayerPhases = false;

            new Interactions.NotifyOnly(PlayerController, string.Format("{0} 获得了胜利", Winner.Name));
            CurrentPhase = "End";
            End();

            CurrentPhase = "PostEnd";
        }

        private bool AreWinnersDecided()
        {
            if (Players.Any(player => player.Health <= 0))
            {
                Winner = OpponentPlayer;
                return true;
            }
            if (Players.Any(player => player.m_library.Count() <= 0))
            {
                Winner = OpponentPlayer;
                return true;
            }

            return false;
        }
    }
}
