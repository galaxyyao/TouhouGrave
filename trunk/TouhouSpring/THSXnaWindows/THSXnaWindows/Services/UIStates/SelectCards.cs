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

        public IIndexable<BaseCard> Selection
        {
            get { return m_selectedCards.ToIndexable(); }
        }

        public Player Player
        {
            get { return m_io.Player; }
        }

        public Interactions.BaseInteraction InteractionObject
        {
            get { return m_io; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            m_io = (Interactions.SelectCards)io;
            m_gameUI.RemoveAllContextButtons();
            m_gameUI.AddContextButton("略过", ContextButton_OnSkip);
            GameApp.Service<ModalDialog>().Show(m_io.Message, () => {});
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            var card = cardControl.Card;

            if (m_io.Candidates.Contains(card))
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

                m_gameUI.RemoveAllContextButtons();
                m_gameUI.AddContextButton(m_selectedCards.Count != 0 ? "确认" : "略过", ContextButton_OnSkip);
            }
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return m_io.Candidates.Contains(cardControl.Card);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return m_selectedCards.Contains(cardControl.Card);
        }

        private void ContextButton_OnSkip(string text)
        {
            m_io.Respond(m_selectedCards.ToIndexable().Clone());
            m_gameUI.LeaveState();
        }
    }
}
