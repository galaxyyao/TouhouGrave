using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    class TacticalPhase : IUIState
    {
        private GameUI m_gameUI = GameApp.Service<GameUI>();
        private Interactions.TacticalPhase m_io;
        private CardInstance[] m_castFromCards;

        public UI.CardControl SelectedCard
        {
            get; private set;
        }

        public bool AttackerSelected
        {
            get; private set;
        }

        public Interactions.BaseInteraction InteractionObject
        {
            get { return m_io; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            m_io = (Interactions.TacticalPhase)io;
            m_gameUI.RemoveAllContextButtons();
            if (m_io.CanPass)
            {
                m_gameUI.AddContextButton("结束", ContextButton_OnPass);
            }
            m_castFromCards = m_io.CastSpellCandidates.Select(spell => spell.Host).Distinct().ToArray();
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            if (AttackerSelected && m_io.DefenderCandidates.Contains(cardControl.CardGuid))
            {
                m_io.RespondAttackCard(
                    m_io.AttackerCandidates.Find(SelectedCard.CardGuid),
                    m_io.DefenderCandidates.Find(cardControl.CardGuid));
                m_gameUI.LeaveState();
                return;
            }

            SelectedCard = SelectedCard == cardControl ? null : cardControl;
            var cardGuid = SelectedCard != null ? SelectedCard.CardGuid : -1;

            m_gameUI.RemoveAllContextButtons();
            if (m_io.CanPass)
            {
                m_gameUI.AddContextButton("结束", ContextButton_OnPass);
            }

            if (m_io.PlayCardCandidates.Contains(cardGuid))
            {
                if (cardControl.CardData.IsWarrior)
                {
                    m_gameUI.AddContextButton("召唤", ContextButton_OnPlay);
                }
                else if (cardControl.CardData.IsInstant)
                {
                    m_gameUI.AddContextButton("施放", ContextButton_OnPlay);
                }
                else if (cardControl.CardData.IsTrap)
                {
                    m_gameUI.AddContextButton("布设", ContextButton_OnPlay);
                }
                else
                {
                    m_gameUI.AddContextButton("Play", ContextButton_OnPlay);
                }
            }
            if (m_io.ActivateAssistCandidates.Contains(cardGuid))
            {
                m_gameUI.AddContextButton("激活", ContextButton_OnActivate);
            }
            if (m_castFromCards.Contains(cardGuid))
            {
                // its okay to access actual CardInstance here because the game is waiting for UI
                var card = m_castFromCards.First(c => c.Guid == cardGuid);
                foreach (var spell in card.Spells.Intersect(m_io.CastSpellCandidates))
                {
                    m_gameUI.AddContextButton("施放 " + spell.Model.Name, text =>
                    {
                        m_io.RespondCast(spell);
                        m_gameUI.LeaveState();
                    });
                }
            }
            if (m_io.SacrificeCandidates.Contains(cardGuid))
            {
                m_gameUI.AddContextButton("牺牲", ContextButton_OnSacrifice);
            }
            if (m_io.RedeemCandidates.Contains(cardGuid))
            {
                m_gameUI.AddContextButton("赎回", ContextButton_OnRedeem);
            }
            AttackerSelected = m_io.AttackerCandidates.Contains(cardGuid);
            if (AttackerSelected && m_io.DefenderCandidates.Count == 0)
            {
                m_gameUI.AddContextButton("攻击对方", ContextButton_OnAttackPlayer);
            }
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            var cardGuid = cardControl.CardGuid;
            return m_io.PlayCardCandidates.Contains(cardGuid)
                   || m_io.ActivateAssistCandidates.Contains(cardGuid)
                   || m_castFromCards.Contains(cardGuid)
                   || m_io.SacrificeCandidates.Contains(cardGuid)
                   || m_io.RedeemCandidates.Contains(cardGuid)
                   || m_io.AttackerCandidates.Contains(cardGuid)
                   || AttackerSelected && m_io.DefenderCandidates.Contains(cardGuid);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return cardControl == SelectedCard;
        }

        private void ContextButton_OnPass(string text)
        {
            m_io.RespondPass();
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnPlay(string text)
        {
            m_io.RespondPlay(m_io.PlayCardCandidates.Find(SelectedCard.CardGuid));
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnActivate(string text)
        {
            m_io.RespondActivate(m_io.ActivateAssistCandidates.Find(SelectedCard.CardGuid));
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnSacrifice(string text)
        {
            m_io.RespondSacrifice(m_io.SacrificeCandidates.Find(SelectedCard.CardGuid));
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnRedeem(string text)
        {
            m_io.RespondRedeem(m_io.RedeemCandidates.Find(SelectedCard.CardGuid));
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnAttackPlayer(string text)
        {
            // TODO: select player to attack
            m_io.RespondAttackPlayer(m_io.AttackerCandidates.Find(SelectedCard.CardGuid), m_io.Player.Game.ActingPlayerEnemies.First());
            m_gameUI.LeaveState();
        }
    }
}
