using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services.UIStates
{
    class PlayerTransition : IUIState
    {
        public Interactions.BaseInteraction InteractionObject
        {
            get { return null; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            var onTurnEnded = io as Interactions.NotifyPlayerEvent;
            onTurnEnded.Respond();
            GameApp.Service<ModalDialog>().Show("", 0, 0, (btn) => { });
        }

        public void OnLeave()
        {
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return false;
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return false;
        }

        public void OnTurnStarted(Interactions.BaseInteraction io)
        {
            var onTurnStarted = io as Interactions.NotifyPlayerEvent;

            GameApp.Service<ModalDialog>().Hide();
            GameApp.Service<ModalDialog>().Show(onTurnStarted.Player.Name + "'s turn", ModalDialog.Button.OK, 0, (btn) =>
            {
                if (onTurnStarted != null && onTurnStarted.Notification == "OnTurnStarted")
                {
                    onTurnStarted.Respond();
                    GameApp.Service<GameUI>().LeaveState();
                }
            });
        }
    }
}
