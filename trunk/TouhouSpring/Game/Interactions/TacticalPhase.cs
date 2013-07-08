using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public partial class TacticalPhase : BaseInteraction
    {
        public struct Result
        {
            public BaseInteraction.PlayerAction ActionType;
            public object Data;
        }

        private IIndexable<CardInstance> m_playCardCandidatesCache;
        private IIndexable<CardInstance> m_activateAssistCandidatesCache;
        private IIndexable<Behaviors.ICastableSpell> m_castSpellCandidatesCache;
        private IIndexable<CardInstance> m_redeemCandidatesCache;
        private IIndexable<CardInstance> m_attackerCandidatesCache;
        private IIndexable<CardInstance> m_defenderCandidatesCache;
        private bool? m_canPassCache;

        public Player Player
        {
            get; private set;
        }

        public IIndexable<CardInstance> PlayCardCandidates
        {
            get
            {
                if (m_playCardCandidatesCache == null)
                {
                    m_playCardCandidatesCache = GetPlayCardCandidates();
                }
                return m_playCardCandidatesCache;
            }
        }

        public IIndexable<CardInstance> ActivateAssistCandidates
        {
            get
            {
                if (m_activateAssistCandidatesCache == null)
                {
                    m_activateAssistCandidatesCache = GetActivateAssistCandidates();
                }
                return m_activateAssistCandidatesCache;
            }
        }

        public IIndexable<Behaviors.ICastableSpell> CastSpellCandidates
        {
            get
            {
                if (m_castSpellCandidatesCache == null)
                {
                    m_castSpellCandidatesCache = GetCastSpellCandidates();
                }
                return m_castSpellCandidatesCache;
            }
        }

        public IIndexable<CardInstance> SacrificeCandidates
        {
            get; private set;
        }

        public IIndexable<CardInstance> RedeemCandidates
        {
            get
            {
                if (m_redeemCandidatesCache == null)
                {
                    m_redeemCandidatesCache = GetRedeemCandidates();
                }
                return m_redeemCandidatesCache;
            }
        }

        public IIndexable<CardInstance> AttackerCandidates
        {
            get
            {
                if (m_attackerCandidatesCache == null)
                {
                    m_attackerCandidatesCache = GetAttackerCandidates();
                }
                return m_attackerCandidatesCache;
            }
        }

        public IIndexable<CardInstance> DefenderCandidates
        {
            get
            {
                if (m_defenderCandidatesCache == null)
                {
                    m_defenderCandidatesCache = GetDefenderCandidates();
                }
                return m_defenderCandidatesCache;
            }
        }

        public bool CanPass
        {
            get
            {
                if (m_canPassCache == null)
                {
                    m_canPassCache = GetCanPass();
                }
                return m_canPassCache.Value;
            }
        }

        internal TacticalPhase(Player player)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (player != player.Game.ActingPlayer)
            {
                throw new InvalidOperationException("TacticalPhase can only be invoked on the acting player.");
            }

            Player = player;

            SacrificeCandidates = !Game.DidSacrifice ? player.CardsOnHand : Indexable.Empty<CardInstance>();
        }

        internal Result Run()
        {
            var result = NotifyAndWait<Result>();
            Validate(result);
            return result;
        }

        public void RespondPass()
        {
            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Pass
            };
            Validate(result);
            RespondBack(result);
        }

        public void RespondPlay(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.PlayCard,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondActivate(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.ActivateAssist,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondCast(Behaviors.ICastableSpell selectedSpell)
        {
            if (selectedSpell == null)
            {
                throw new ArgumentNullException("selectedSpell");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.CastSpell,
                Data = selectedSpell
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondSacrifice(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Sacrifice,
                Data = selectedCard
            };
            
            Validate(result);
            RespondBack(result);
        }

        public void RespondRedeem(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Redeem,
                Data = selectedCard
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAttackCard(CardInstance attacker, CardInstance defender)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (defender == null)
            {
                throw new ArgumentNullException("defender");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.AttackCard,
                Data = new CardInstance[] { attacker, defender }
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAttackPlayer(CardInstance attacker, Player player)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            var result = new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.AttackPlayer,
                Data = new object[] { attacker, player }
            };

            Validate(result);
            RespondBack(result);
        }

        public void RespondAbort()
        {
            RespondBack(new Result { ActionType = Interactions.BaseInteraction.PlayerAction.Abort });
        }

        protected void Validate(Result result)
        {
            switch (result.ActionType)
            {
                case Interactions.BaseInteraction.PlayerAction.Pass:
                    if (!CanPass)
                    {
                        throw new InteractionValidationFailException("Can't pass.");
                    }
                    else if (result.Data != null)
                    {
                        throw new InteractionValidationFailException("Action Pass shall have null data.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.PlayCard:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action PlayCard shall have an object of CardInstance as its data.");
                    }
                    else if (!PlayCardCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be played.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.ActivateAssist:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action ActivateAssist shall have an object of CardInstance as its data.");
                    }
                    else if (!ActivateAssistCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected assist card can't be activated.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.CastSpell:
                    if (!(result.Data is Behaviors.ICastableSpell))
                    {
                        throw new InteractionValidationFailException("Action CastSpell shall have an object of ICastableSpell as its data.");
                    }
                    if (!CastSpellCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected spell can't be casted.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.Sacrifice:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action Sacrifice shall have an object of CardInstance as its data.");
                    }
                    else if (!SacrificeCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be sacrificed.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.Redeem:
                    if (!(result.Data is CardInstance))
                    {
                        throw new InteractionValidationFailException("Action Redeem shall have an object of CardInstance as its data.");
                    }
                    else if (!RedeemCandidates.Contains(result.Data))
                    {
                        throw new InteractionValidationFailException("Selected card can't be redeemed.");
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.AttackCard:
                    {
                        var pair = result.Data as CardInstance[];
                        if (pair == null || pair.Length != 2)
                        {
                            throw new InteractionValidationFailException("Action AttackCard shall have a pair of CardInstances as its data.");
                        }
                        else if (!AttackerCandidates.Contains(pair[0]))
                        {
                            throw new InteractionValidationFailException("Attacking card can't attack.");
                        }
                        else if (!DefenderCandidates.Contains(pair[1]))
                        {
                            throw new InteractionValidationFailException("Defending card can't defend.");
                        }
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.AttackPlayer:
                    {
                        var pair = result.Data as object[];
                        if (pair == null || pair.Length != 2 || !(pair[0] is CardInstance) || !(pair[1] is Player))
                        {
                            throw new InteractionValidationFailException("Action AttackPlayer shall have a pair of CardInstance and Player as its data.");
                        }
                        else if (!AttackerCandidates.Contains(pair[0]))
                        {
                            throw new InteractionValidationFailException("Attacking card can't attack.");
                        }
                        else if (pair[1] == Player)
                        {
                            throw new InteractionValidationFailException("Player can't be attacked.");
                        }
                    }
                    break;

                case Interactions.BaseInteraction.PlayerAction.Abort:
                    break;

                default:
                    throw new InteractionValidationFailException("Invalid action performed.");
            }
        }

        private IIndexable<CardInstance> GetPlayCardCandidates()
        {
            return GetPlayCardBaseSet(Player).Where(card => Player.Game.IsCardPlayable(card)).ToList().ToIndexable();
        }

        private IIndexable<CardInstance> GetActivateAssistCandidates()
        {
            return GetActivateAssistBaseSet(Player).Where(card => Player.Game.IsCardActivatable(card)).ToList().ToIndexable();
        }

        private IIndexable<Behaviors.ICastableSpell> GetCastSpellCandidates()
        {
            return GetCastSpellBaseSet(Player).SelectMany(card => card.Spells).Where(spell => Player.Game.IsSpellCastable(spell)).ToList().ToIndexable();
        }

        private IIndexable<CardInstance> GetRedeemCandidates()
        {
            return !Game.DidRedeem
                   ? Player.CardsSacrificed.Where(card => Player.Game.IsCardRedeemable(card)).ToList().ToIndexable()
                   : Indexable.Empty<CardInstance>();
        }

        private IIndexable<CardInstance> GetAttackerCandidates()
        {
            return GetAttackerBaseSet(Player).Where(card => !card.Behaviors.Has<Behaviors.IUnattackable>()).ToList().ToIndexable();
        }

        private IIndexable<CardInstance> GetDefenderCandidates()
        {
            if (AttackerCandidates.Count != 0)
            {
                var defendersArr = GetDefenderBaseSet(Player).Where(card => !card.Behaviors.Has<Behaviors.IUndefendable>()).ToList();
                var protectorsArr = defendersArr.Where(card => card.Behaviors.Has<Behaviors.ITaunt>()).ToList();
                if (protectorsArr.Count == 0)
                {
                    protectorsArr = defendersArr.Where(card => card.Behaviors.Has<Behaviors.IProtector>()).ToList();
                }
                return (protectorsArr.Count != 0 ? protectorsArr : defendersArr).ToIndexable();
            }
            else
            {
                return Indexable.Empty<CardInstance>();
            }
        }

        private bool GetCanPass()
        {
            return AttackerCandidates.Count == 0
                   || !AttackerCandidates.Any(card => card.Behaviors.Has<Behaviors.IForceAttack>())
                      && !DefenderCandidates.Any(card => card.Behaviors.Has<Behaviors.ITaunt>());
        }

        private static IEnumerable<CardInstance> GetPlayCardBaseSet(Player player)
        {
            if (!player.CardsOnBattlefield.Contains(player.Hero) && player.Hero != null)
            {
                yield return player.Hero;
            }
            foreach (var card in player.CardsOnHand)
            {
                yield return card;
            }
        }

        private static IEnumerable<CardInstance> GetActivateAssistBaseSet(Player player)
        {
            foreach (var card in player.Assists)
            {
                if (!player.ActivatedAssits.Contains(card))
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<CardInstance> GetCastSpellBaseSet(Player player)
        {
            foreach (var card in player.ActivatedAssits)
            {
                yield return card;
            }
            foreach (var card in player.CardsOnBattlefield)
            {
                yield return card;
            }
        }

        private static IEnumerable<CardInstance> GetAttackerBaseSet(Player player)
        {
            foreach (var card in player.CardsOnBattlefield)
            {
                if (card.Warrior != null && card.Warrior.State == Behaviors.WarriorState.StandingBy)
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<CardInstance> GetDefenderBaseSet(Player player)
        {
            foreach (var p in player.Game.Players)
            {
                if (p != player)
                {
                    foreach (var card in p.CardsOnBattlefield)
                    {
                        if (card.Warrior != null)
                        {
                            yield return card;
                        }
                    }
                }
            }
        }
    }
}
