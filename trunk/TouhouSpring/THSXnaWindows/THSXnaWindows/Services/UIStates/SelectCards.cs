using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    class SelectCards : IUIState
    {
        private List<BaseCard> m_selectedCards = new List<BaseCard>();

        private GameUI m_gameUI = GameApp.Service<GameUI>();
        private Interactions.SelectCards m_io;

        public Interactions.BaseInteraction InteractionObject
        {
            get { return m_io; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            m_io = (Interactions.SelectCards)io;
            GameApp.Service<ModalDialog>().Show(m_io.Message, () =>
            {
                m_gameUI.SetSinglePhaseButton(GameUI.PhaseButtonText.Skip);
            });
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            var card = cardControl.Card;

            if (m_io.SelectFromSet.Contains(card))
            {
                if (m_selectedCards.Contains(card))
                {
                    m_selectedCards.Remove(card);
                }
                else
                {
                    if (m_io.Mode == Interactions.SelectCards.SelectMode.Single)
                    {
                        m_selectedCards.Clear();
                    }
                    m_selectedCards.Add(card);
                }

                m_gameUI.SetSinglePhaseButton(m_selectedCards.Count != 0 ? GameUI.PhaseButtonText.Done : GameUI.PhaseButtonText.Skip);
            }
        }

        public void OnSpellClicked(UI.CardControl cardControl, Behaviors.ICastableSpell spell)
        {
            throw new InvalidOperationException("Impossible");
        }

        public void OnPhaseButton(GameUI.PhaseButtonText buttonText)
        {
            m_io.Respond(m_selectedCards.ToIndexable().Clone());
            m_gameUI.LeaveState();
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return m_io.SelectFromSet.Contains(cardControl.Card);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return m_selectedCards.Contains(cardControl.Card);
        }

        public bool IsCardSelectedForCastSpell(UI.CardControl cardControl)
        {
            return false;
        }
    }
}
