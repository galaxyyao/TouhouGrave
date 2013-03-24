using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    class SelectCards : IUIState
    {
        private List<int> m_selectedCards = new List<int>();

        private GameUI m_gameUI = GameApp.Service<GameUI>();
        private Interactions.SelectCards m_io;

        public IIndexable<int> Selection
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
            GameApp.Service<PopupDialog>().PushMessageBox(m_io.Message);
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            var cardGuid = cardControl.CardGuid;

            if (m_io.Candidates.Contains(cardGuid))
            {
                if (m_selectedCards.Contains(cardGuid))
                {
                    m_selectedCards.Remove(cardGuid);
                }
                else
                {
                    if (m_io.Mode == Interactions.SelectCards.SelectMode.Single)
                    {
                        m_selectedCards.Clear();
                    }
                    m_selectedCards.Add(cardGuid);
                }

                m_gameUI.RemoveAllContextButtons();
                m_gameUI.AddContextButton(m_selectedCards.Count != 0 ? "确认" : "略过", ContextButton_OnSkip);
            }
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return m_io.Candidates.Contains(cardControl.CardGuid);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return m_selectedCards.Contains(cardControl.CardGuid);
        }

        private void ContextButton_OnSkip(string text)
        {
            m_io.Respond(m_selectedCards.MapToCards(m_io.Candidates).ToIndexable());
            m_gameUI.LeaveState();
        }
    }
}
