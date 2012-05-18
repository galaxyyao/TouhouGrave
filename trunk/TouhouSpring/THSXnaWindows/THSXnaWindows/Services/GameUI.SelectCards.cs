using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private List<BaseCard> m_selectedCards = new List<BaseCard>();

        public void SelectCards_Enter(Interactions.SelectCards io)
        {
            GameApp.Service<ModalDialog>().Show(io.Message, () =>
            {
                SetNextButton(NextButton.Skip);
            });
        }

        private void SelectCards_OnNextButton(Interactions.SelectCards io)
        {
            io.Respond(m_selectedCards.ToIndexable().Clone());
            m_selectedCards.Clear();
        }

        private void SelectCards_OnCardClicked(UI.CardControl control, Interactions.SelectCards io)
        {
            var card = control.Card;

            if (io.FromSet.Contains(card))
            {
                if (m_selectedCards.Contains(card))
                {
                    m_selectedCards.Remove(card);
                }
                else
                {
                    if (io.Mode == Interactions.SelectCards.SelectMode.Single)
                    {
                        m_selectedCards.Clear();
                    }
                    m_selectedCards.Add(card);
                }

                SetNextButton(m_selectedCards.Count != 0 ? NextButton.Done : NextButton.Skip);
            }
        }

        private bool SelectCards_ShouldBeHighlighted(BaseCard card)
        {
            return m_selectedCards.Contains(card);
        }
    }
}
