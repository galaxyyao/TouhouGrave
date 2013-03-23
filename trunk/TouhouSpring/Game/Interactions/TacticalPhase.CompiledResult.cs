using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public partial class TacticalPhase
    {
        public abstract class CompiledResponse
        {
            public abstract Result Restore(Player actingPlayer);
        }

        private class CompiledPass : CompiledResponse
        {
            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result { ActionType = Interactions.BaseInteraction.PlayerAction.Pass };
            }
        }

        private class CompiledPlayCard : CompiledResponse
        {
            private Int32 m_cardGuid;
            public CompiledPlayCard(CardInstance card)
            {
                m_cardGuid = card.Guid;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.PlayCard,
                    Data = GetPlayCardBaseSet(actingPlayer).First(card => card.Guid == m_cardGuid)
                };
            }
        }

        private class CompiledActivateAssist : CompiledResponse
        {
            private Int32 m_cardGuid;
            public CompiledActivateAssist(CardInstance card)
            {
                m_cardGuid = card.Guid;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.ActivateAssist,
                    Data = GetActivateAssistBaseSet(actingPlayer).First(card => card.Guid == m_cardGuid)
                };
            }
        }

        private class CompiledCastSpell : CompiledResponse
        {
            private Int32 m_hostGuid;
            private int m_bhvIndex;
            public CompiledCastSpell(Behaviors.ICastableSpell spell)
            {
                m_hostGuid = spell.Host.Guid;
                m_bhvIndex = spell.Host.Behaviors.IndexOf(spell);
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.CastSpell,
                    Data = GetCastSpellBaseSet(actingPlayer).First(card => card.Guid == m_hostGuid)
                           .Behaviors[m_bhvIndex]
                };
            }
        }

        private class CompiledSacrifice : CompiledResponse
        {
            private Int32 m_cardGuid;
            public CompiledSacrifice(CardInstance card)
            {
                m_cardGuid = card.Guid;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.Sacrifice,
                    Data = actingPlayer.CardsOnHand.First(card => card.Guid == m_cardGuid)
                };
            }
        }

        private class CompiledRedeem : CompiledResponse
        {
            private Int32 m_cardGuid;
            public CompiledRedeem(CardInstance card)
            {
                m_cardGuid = card.Guid;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.Redeem,
                    Data = actingPlayer.CardsSacrificed.First(card => card.Guid == m_cardGuid)
                };
            }
        }

        private class CompiledAttackCard : CompiledResponse
        {
            private Int32 m_attackerGuid;
            private Int32 m_defenderGuid;
            public CompiledAttackCard(CardInstance attacker, CardInstance defender)
            {
                m_attackerGuid = attacker.Guid;
                m_defenderGuid = defender.Guid;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.AttackCard,
                    Data = new CardInstance[]
                    {
                        GetAttackerBaseSet(actingPlayer).First(card => card.Guid == m_attackerGuid),
                        GetDefenderBaseSet(actingPlayer).First(card => card.Guid == m_defenderGuid)
                    }
                };
            }
        }

        private class CompiledAttackPlayer : CompiledResponse
        {
            private Int32 m_attackerGuid;
            private int m_playerIndex;
            public CompiledAttackPlayer(CardInstance attacker, Player player)
            {
                m_attackerGuid = attacker.Guid;
                m_playerIndex = player.Index;
            }

            public override Result Restore(Player actingPlayer)
            {
                if (actingPlayer == null)
                {
                    throw new ArgumentNullException("actingPlayer");
                }

                return new Result
                {
                    ActionType = Interactions.BaseInteraction.PlayerAction.AttackPlayer,
                    Data = new object[]
                    {
                        GetAttackerBaseSet(actingPlayer).First(card => card.Guid == m_attackerGuid),
                        actingPlayer.Game.Players[m_playerIndex]
                    }
                };
            }
        }

        public CompiledResponse CompiledRespondPass()
        {
            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Pass
            });

            return new CompiledPass();
        }

        public CompiledResponse CompiledRespondPlay(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.PlayCard,
                Data = selectedCard
            });

            return new CompiledPlayCard(selectedCard);
        }

        public CompiledResponse CompiledRespondActivate(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.ActivateAssist,
                Data = selectedCard
            });

            return new CompiledActivateAssist(selectedCard);
        }

        public CompiledResponse CompiledRespondCast(Behaviors.ICastableSpell selectedSpell)
        {
            if (selectedSpell == null)
            {
                throw new ArgumentNullException("selectedSpell");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.CastSpell,
                Data = selectedSpell
            });

            return new CompiledCastSpell(selectedSpell);
        }

        public CompiledResponse CompiledRespondSacrifice(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Sacrifice,
                Data = selectedCard
            });

            return new CompiledSacrifice(selectedCard);
        }

        public CompiledResponse CompiledRespondRedeem(CardInstance selectedCard)
        {
            if (selectedCard == null)
            {
                throw new ArgumentNullException("selectedCard");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.Redeem,
                Data = selectedCard
            });

            return new CompiledRedeem(selectedCard);
        }

        public CompiledResponse CompiledRespondAttackCard(CardInstance attacker, CardInstance defender)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (defender == null)
            {
                throw new ArgumentNullException("defender");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.AttackCard,
                Data = new CardInstance[] { attacker, defender }
            });

            return new CompiledAttackCard(attacker, defender);
        }

        public CompiledResponse CompiledRespondAttackPlayer(CardInstance attacker, Player player)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            else if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Validate(new Result
            {
                ActionType = Interactions.BaseInteraction.PlayerAction.AttackPlayer,
                Data = new object[] { attacker, player }
            });

            return new CompiledAttackPlayer(attacker, player);
        }
    }
}
