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
        private BaseCard[] m_castFromCards;

        public UI.CardControl SelectedCard
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
            m_gameUI.AddContextButton("Pass", ContextButton_OnPass);
            m_castFromCards = m_io.CastSpellCandidates.Select(spell => spell.Host).Distinct().ToArray();
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            SelectedCard = SelectedCard == cardControl ? null : cardControl;
            var card = SelectedCard != null ? SelectedCard.Card : null;

            m_gameUI.RemoveAllContextButtons();
            if (card == null)
            {
                m_gameUI.AddContextButton("Pass", ContextButton_OnPass);
            }

            if (m_io.PlayCardCandidates.Contains(card))
            {
                if (card.Behaviors.Has<Behaviors.Warrior>())
                {
                    m_gameUI.AddContextButton("Deploy", ContextButton_OnPlay);
                }
                else if (card.Behaviors.Has<Behaviors.Assist>())
                {
                    m_gameUI.AddContextButton("Activate", ContextButton_OnPlay);
                }
                else if (card.Behaviors.Has<Behaviors.Instant>())
                {
                    m_gameUI.AddContextButton("Cast", ContextButton_OnPlay);
                }
                else
                {
                    m_gameUI.AddContextButton("Play", ContextButton_OnPlay);
                }
            }
            if (m_castFromCards.Contains(card))
            {
                foreach (var spell in card.Spells.Intersect(m_io.CastSpellCandidates))
                {
                    m_gameUI.AddContextButton("Cast " + spell.Model.Name, text =>
                    {
                        m_io.Respond(spell);
                        m_gameUI.LeaveState();
                    });
                }
            }
            if (m_io.SacrificeCandidates.Contains(card))
            {
                m_gameUI.AddContextButton("Sacrifice", ContextButton_OnPlay);
            }
            if (m_io.RedeemCandidates.Contains(card))
            {
                m_gameUI.AddContextButton("Redeem", ContextButton_OnPlay);
            }
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            var card = cardControl.Card;
            return m_io.PlayCardCandidates.Contains(card)
                   || m_castFromCards.Contains(card)
                   || m_io.SacrificeCandidates.Contains(card)
                   || m_io.RedeemCandidates.Contains(card);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return cardControl == SelectedCard;
        }

        private void ContextButton_OnPass(string text)
        {
            m_io.Respond();
            m_gameUI.LeaveState();
        }

        private void ContextButton_OnPlay(string text)
        {
            m_io.Respond(SelectedCard.Card);
            m_gameUI.LeaveState();
        }
    }
}
