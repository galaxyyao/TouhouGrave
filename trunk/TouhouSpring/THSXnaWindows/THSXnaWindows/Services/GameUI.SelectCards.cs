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
                SetSinglePhaseButton(PhaseButtonText.Skip);
            });
        }

        private bool SelectCards_OnPhaseButton(Interactions.SelectCards io, PhaseButtonText buttonText)
        {
            io.Respond(m_selectedCards.ToIndexable().Clone());
            m_selectedCards.Clear();
            return true;
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

                SetSinglePhaseButton(m_selectedCards.Count != 0 ? PhaseButtonText.Done : PhaseButtonText.Skip);
            }
        }

        private bool SelectCards_ShouldBeHighlighted(BaseCard card)
        {
            return m_selectedCards.Contains(card);
        }
    }
}
