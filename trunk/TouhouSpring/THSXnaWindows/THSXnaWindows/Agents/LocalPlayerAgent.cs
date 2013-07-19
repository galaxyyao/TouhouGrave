using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class LocalPlayerAgent : BaseAgent
    {
        public override bool OnCardPlayCanceled(Interactions.NotifyCardEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.PopupDialog>().PushMessageBox(io.Message, () => io.Respond());
                return true;
            }

            return false;
        }

        public override bool OnSpellCastCanceled(Interactions.NotifySpellEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.PopupDialog>().PushMessageBox(io.Message, () => io.Respond());
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
            var nextPid = io.Game.NextActingPlayer.Index;
            if ((io.Game.Controller as XnaUIController).Agents[nextPid] is LocalPlayerAgent)
            {
                GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.PlayerTransition(), io);
                return true;
            }
            return false;
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
            // translate from Interactions.MessageBox.Button to UI.ModalDialogs.MessageBox.ButtonFlags
            UI.ModalDialogs.MessageBox.ButtonFlags buttons = 0;
            if ((io.Buttons & Interactions.MessageBoxButtons.OK) != 0)
            {
                buttons |= UI.ModalDialogs.MessageBox.ButtonFlags.OK;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Cancel) != 0)
            {
                buttons |= UI.ModalDialogs.MessageBox.ButtonFlags.Cancel;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Yes) != 0)
            {
                buttons |= UI.ModalDialogs.MessageBox.ButtonFlags.Yes;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.No) != 0)
            {
                buttons |= UI.ModalDialogs.MessageBox.ButtonFlags.No;
            }
            GameApp.Service<Services.PopupDialog>().PushMessageBox(io.Text, buttons, btn =>
            {
                // translate back...
                Interactions.MessageBoxButtons ibtn;
                switch (btn)
                {
                    case UI.ModalDialogs.MessageBox.ButtonOK:
                        ibtn = Interactions.MessageBoxButtons.OK;
                        break;
                    case UI.ModalDialogs.MessageBox.ButtonCancel:
                        ibtn = Interactions.MessageBoxButtons.Cancel;
                        break;
                    case UI.ModalDialogs.MessageBox.ButtonYes:
                        ibtn = Interactions.MessageBoxButtons.Yes;
                        break;
                    case UI.ModalDialogs.MessageBox.ButtonNo:
                        ibtn = Interactions.MessageBoxButtons.No;
                        break;
                    default:
                        throw new ArgumentException("btn");
                }
                io.Respond(ibtn);
            });
        }

        public override void OnSelectNumber(Interactions.SelectNumber io)
        {
            GameApp.Service<Services.PopupDialog>().PushNumberSelector(io.Message, io.Minimum, io.Maximum,
            (btn, value) =>
            {
                io.Respond(btn == UI.ModalDialogs.NumberSelector.ButtonOK ? (int?)value : null);
            });
        }

        public override void OnSelectCardModel(Interactions.SelectCardModel io)
        {
            GameApp.Service<Services.PopupDialog>().PushCardModelSelector(io.Message, io.Candidates, card =>
            {
                io.Respond(card);
            });
        }
    }
}
