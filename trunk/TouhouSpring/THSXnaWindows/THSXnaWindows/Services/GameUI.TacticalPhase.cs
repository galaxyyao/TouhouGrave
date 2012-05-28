using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        public void TacticalPhase_Enter(Interactions.TacticalPhase io)
        {
            SetNextButton(NextButton.Skip);
            SetDrawCardButton(DrawCardButton.ShowButton);
        }

        public void TacticalPhase_Leave()
        {
            SetDrawCardButton(DrawCardButton.HideButton);
            TacticalPhase_CardToPlay = null;
            TacticalPhase_CardToCastSpell = null;
        }

        public BaseCard TacticalPhase_CardToPlay
        {
            get;
            private set;
        }

        public BaseCard TacticalPhase_CardToCastSpell
        {
            get;
            private set;
        }

        private void TacticalPhase_OnCardClicked(UI.CardControl control, Interactions.TacticalPhase io)
        {
            var card = control.Card;

            if (io.FromSet.Contains(card))
            {
                TacticalPhase_CardToPlay = (card != TacticalPhase_CardToPlay) ? card : null;
                TacticalPhase_CardToCastSpell = null;
                SetNextButton(TacticalPhase_CardToPlay != null ? NextButton.Done : NextButton.Skip);
            }
            else if (io.ComputeCastFromSet().Contains(card))
            {
                TacticalPhase_CardToPlay = null;
                TacticalPhase_CardToCastSpell = (card != TacticalPhase_CardToCastSpell) ? card : null;
            }
        }

        private void TacticalPhase_OnNextButton(Interactions.TacticalPhase io)
        {
            io.Respond(TacticalPhase_CardToPlay);
            if (TacticalPhase_CardToPlay == null)
            {
                TacticalPhase_Leave();
            }
        }

        private bool TacticalPhase_ShouldBeHighlighted(BaseCard card)
        {
            return card == TacticalPhase_CardToPlay;
        }

        private void TacticalPhase_OnDrawCardButton(Interactions.TacticalPhase io)
        {
            io.Respond(Game.PlayerPlayer);
        }
    }
}
