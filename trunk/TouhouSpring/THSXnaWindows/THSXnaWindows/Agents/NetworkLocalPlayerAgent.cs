using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Network = TouhouSpring.Network;

namespace TouhouSpring.Agents
{
    class NetworkLocalPlayerAgent : BaseAgent
    {
        private Network.Client m_NetworkClient = null;

        public NetworkLocalPlayerAgent()
        {
            m_NetworkClient = GameApp.Service<Services.Network>().THSClient;
        }

        public override bool OnCardPlayCanceled(Interactions.NotifyCardEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.PopupDialog>().PushMessageBox(io.Message, () =>
                {
                    io.Respond();
                });
                return true;
            }

            return false;
        }

        public override bool OnSpellCastCanceled(Interactions.NotifySpellEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.PopupDialog>().PushMessageBox(io.Message, () =>
                    {
                        io.Respond();
                    });
                return true;
            }

            return false;
        }

        public override bool OnTurnEnded(Interactions.NotifyPlayerEvent io)
        {
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

        public override void OnInitiativeCommandEnd()
        {
            // flush the response queue thru network interface
        }

        public override void OnInitiativeCommandCanceled()
        {
            // clear the response queue
        }

        public override void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            if (io is Interactions.TacticalPhase)
            {
                var tacticalPhaseIo = (io as Interactions.TacticalPhase);
                var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
                if (tacticalPhaseResult.ActionType == Interactions.TacticalPhase.Action.Pass)
                {
                    m_NetworkClient.QueueLocalMessage("switchturn");
                }
                // queue


                // if the response is AttackCard, AttackPlayer, Pass
                // flush
                if (tacticalPhaseResult.ActionType == Interactions.TacticalPhase.Action.AttackCard
                    || tacticalPhaseResult.ActionType == Interactions.TacticalPhase.Action.AttackPlayer
                    || tacticalPhaseResult.ActionType == Interactions.TacticalPhase.Action.Pass
                    )
                    m_NetworkClient.FlushLocalMessage();

            }
            else if (io is Interactions.SelectCards
                || io is Interactions.SelectNumber
                || io is Interactions.MessageBox)
            {
                // queue

                if (io.Game.RunningCommand.ExecutionPhase != Commands.CommandPhase.Condition)
                {
                    // means the command will never be canceled
                    // flush
                }
            }
        }

    }
}
