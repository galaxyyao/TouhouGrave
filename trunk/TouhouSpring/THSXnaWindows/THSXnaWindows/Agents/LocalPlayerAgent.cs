using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class LocalPlayerAgent : BaseAgent
    {
        public override bool OnCardPlayCanceled(Interactions.NotifyCardEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.ModalDialog>().Show(io.Message, () => io.Respond());
                return true;
            }

            return false;
        }

        public override bool OnSpellCastCanceled(Interactions.NotifySpellEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.ModalDialog>().Show(io.Message, () => io.Respond());
                return true;
            }

            return false;
        }

        public override bool OnTurnStarted(Interactions.NotifyPlayerEvent io)
        {
            if (GameApp.Service<Services.GameUI>().UIState is Services.UIStates.PlayerTransition)
            {
                (GameApp.Service<Services.GameUI>().UIState as Services.UIStates.PlayerTransition).OnTurnStarted(io);
                return true;
            }

            return false;
        }

        public override bool OnTurnEnded(Interactions.NotifyPlayerEvent io)
        {
            GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.PlayerTransition(), io);
            return true;
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.TacticalPhase(), io);
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.SelectCards(), io);
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            // translate from Interactions.MessageBox.Button to Services.ModalDialog.Button
            Services.ModalDialog.Button buttons = 0;
            if ((io.Buttons & Interactions.MessageBoxButtons.OK) != 0)
            {
                buttons |= Services.ModalDialog.Button.OK;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Cancel) != 0)
            {
                buttons |= Services.ModalDialog.Button.Cancel;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Yes) != 0)
            {
                buttons |= Services.ModalDialog.Button.Yes;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.No) != 0)
            {
                buttons |= Services.ModalDialog.Button.No;
            }
            GameApp.Service<Services.ModalDialog>().Show(io.Text, buttons, btn =>
            {
                // translate back...
                Interactions.MessageBoxButtons ibtn;
                switch (btn)
                {
                    case Services.ModalDialog.Button.OK:
                        ibtn = Interactions.MessageBoxButtons.OK;
                        break;
                    case Services.ModalDialog.Button.Cancel:
                        ibtn = Interactions.MessageBoxButtons.Cancel;
                        break;
                    case Services.ModalDialog.Button.Yes:
                        ibtn = Interactions.MessageBoxButtons.Yes;
                        break;
                    case Services.ModalDialog.Button.No:
                        ibtn = Interactions.MessageBoxButtons.No;
                        break;
                    default:
                        throw new ArgumentException("btn");
                }
                io.Respond(ibtn);
            });
        }
    }
}
