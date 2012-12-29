using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    class TacticalPhase : IUIState
    {
        private UI.CardControl m_spellToCastCard;

        private GameUI m_gameUI = GameApp.Service<GameUI>();
        private Interactions.TacticalPhase m_io;
        private BaseCard[] m_castFromCards;

        public Interactions.BaseInteraction InteractionObject
        {
            get { return m_io; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            m_io = (Interactions.TacticalPhase)io;
            m_gameUI.SetContextButtons("Pass");
            m_castFromCards = m_io.CastSpellCandidates.Select(spell => spell.Host).Distinct().ToArray();
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            var card = cardControl.Card;

            if (m_io.PlayCardCandidates.Contains(card))
            {
                GameApp.Service<ModalDialog>().Show(
                    String.Format(CultureInfo.CurrentCulture, "Play {0}?", card.Model.Name),
                    ModalDialog.Button.Yes | ModalDialog.Button.Cancel,
                    btn =>
                    {
                        if (btn == ModalDialog.Button.Yes)
                        {
                            m_io.Respond(card);
                            m_gameUI.LeaveState();
                        }
                    });
            }
            else if (m_castFromCards.Contains(card))
            {
                m_spellToCastCard = (cardControl != m_spellToCastCard) ? cardControl : null;
            }
            else
            {
                return;
            }
        }

        public void OnSpellClicked(UI.CardControl cardControl, Behaviors.ICastableSpell spell)
        {
            GameApp.Service<ModalDialog>().Show(
                String.Format(CultureInfo.CurrentCulture, "Cast {0}?", spell.Model.Name),
                ModalDialog.Button.Yes | ModalDialog.Button.Cancel,
                btn =>
                {
                    if (btn == ModalDialog.Button.Yes)
                    {
                        m_io.Respond(spell);
                        m_gameUI.LeaveState();
                    }
                });
        }

        public void OnContextButton(string buttonText)
        {
            if (buttonText == "Pass")
            {
                m_io.Respond();
            }
            else
            {
                throw new InvalidOperationException("Impossible");
            }
            m_gameUI.LeaveState();
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
            return false;
        }

        public bool IsCardSelectedForCastSpell(UI.CardControl cardControl)
        {
            return cardControl == m_spellToCastCard;
        }
    }
}
