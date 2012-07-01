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
            SetSinglePhaseButton(PhaseButtonText.Skip);
            AddPhaseButton(PhaseButtonText.Draw);
        }

        public void TacticalPhase_Leave()
        {
            TacticalPhase_CardToPlay = null;
            TacticalPhase_CardToCastSpell = null;
        }

        public BaseCard TacticalPhase_CardToPlay
        {
            get; private set;
        }

        public BaseCard TacticalPhase_CardToCastSpell
        {
            get; private set;
        }

        private void TacticalPhase_OnCardClicked(UI.CardControl control, Interactions.TacticalPhase io)
        {
            var card = control.Card;

            if (io.FromSet.Contains(card))
            {
                TacticalPhase_CardToPlay = (card != TacticalPhase_CardToPlay) ? card : null;
                TacticalPhase_CardToCastSpell = null;

                SetSinglePhaseButton(TacticalPhase_CardToPlay != null ? PhaseButtonText.Done : PhaseButtonText.Skip);
                AddPhaseButton(PhaseButtonText.Draw);
            }
            else if (io.ComputeCastFromSet().Contains(card))
            {
                TacticalPhase_CardToPlay = null;
                TacticalPhase_CardToCastSpell = (card != TacticalPhase_CardToCastSpell) ? card : null;
            }
        }

        private bool TacticalPhase_OnPhaseButton(Interactions.TacticalPhase io, PhaseButtonText buttonText)
        {
            if (buttonText == PhaseButtonText.Done)
            {
                io.Respond(TacticalPhase_CardToPlay);
            }
            else if (buttonText == PhaseButtonText.Skip)
            {
                io.Respond((BaseCard)null);
                TacticalPhase_Leave();
            }
            else if (buttonText == PhaseButtonText.Draw)
            {
                if (Game.PlayerPlayer.Mana < 1)
                {
                    GameApp.Service<ModalDialog>().Show("Not sufficient mana.", ModalDialog.Button.OK, btn => { });
                    return false;
                }
                else
                {
                    io.RespondDraw();
                }
            }
            return true;
        }

        private bool TacticalPhase_ShouldBeHighlighted(BaseCard card)
        {
            return card == TacticalPhase_CardToPlay;
        }
    }
}
