﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Services;
using TouhouSpring.UI;

namespace TouhouSpring
{
	class XnaUIController : BaseController
	{
		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyOnly))]
		private bool OnNotified(Interactions.NotifyOnly interactionObj)
		{
			throw new InvalidOperationException("NotifyOnly shall not occur.");
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyCardEvent))]
		private bool OnNotified(Interactions.NotifyCardEvent interactionObj)
		{
            switch (interactionObj.Notification)
            {
                case "OnCardAddedToManaPool":
                case "OnCardDrawn":
                case "OnCardSummoned":
                    GameApp.Service<GameUI>().RegisterCard(interactionObj.Card);
                    break;
                case "OnCardDestroyed":
                    GameApp.Service<GameUI>().UnregisterCard(interactionObj.Card);
                    break;
                case "OnCardPlayCanceled":
                    // TODO: (Controller) Show dialog only to Local agent
                    if (interactionObj.Message != string.Empty)
                    {
                        GameApp.Service<Services.ModalDialog>().Show(interactionObj.Message, () => interactionObj.Respond());
                        return true;
                    }
                    break;
                default:
                    break;
            }

			interactionObj.Respond();
			return false;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyGameEvent))]
		private bool OnNotified(Interactions.NotifyGameEvent interactionObj)
		{
			switch (interactionObj.Notification)
			{
				default:
					break;
			}

			interactionObj.Respond();
			return false;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyPlayerEvent))]
		private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
		{
			switch (interactionObj.Notification)
			{
				case "OnPlayerDamaged":
					break;
                case "OnTurnEnded":
                    GameApp.Service<GameUI>().EnterState(new Services.UIStates.PlayerTransition(), interactionObj);
                    return true;
                case "OnTurnStarted":
                    if (GameApp.Service<GameUI>().UIState is Services.UIStates.PlayerTransition)
                    {
                        (GameApp.Service<GameUI>().UIState as Services.UIStates.PlayerTransition).OnTurnStarted(interactionObj);
                        return true;
                    }
                    break;
				default:
					break;
			}

			interactionObj.Respond();
			return false;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifySpellEvent))]
		private bool OnNotified(Interactions.NotifySpellEvent interactionObj)
		{
            switch (interactionObj.Notification)
            {
                case "OnSpellCasted":
                    break;
                case "OnSpellCastCanceled":
                    // TODO: (Controller) Show dialog only to Local agent
                    GameApp.Service<Services.ModalDialog>().Show(interactionObj.Message, () =>
                    {
                        interactionObj.Respond();
                    });
                    return true;
                default:
                    break;
            }

			interactionObj.Respond();
			return false;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.TacticalPhase))]
		private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
		{
            GameApp.Service<GameUI>().EnterState(new Services.UIStates.TacticalPhase(), interactionObj);
			return true;
		}

        [Interactions.MessageMap.Handler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            GameApp.Service<GameUI>().EnterState(new Services.UIStates.SelectCards(), interactionObj);
            return true;
        }

		[Interactions.MessageMap.Handler(typeof(Interactions.MessageBox))]
		private bool OnMessageBox(Interactions.MessageBox interactionObj)
		{
			// translate from Interactions.MessageBox.Button to Services.ModalDialog.Button
			Services.ModalDialog.Button buttons = 0;
			if ((interactionObj.Buttons & Interactions.MessageBoxButtons.OK) != 0)
			{
				buttons |= Services.ModalDialog.Button.OK;
			}
			if ((interactionObj.Buttons & Interactions.MessageBoxButtons.Cancel) != 0)
			{
				buttons |= Services.ModalDialog.Button.Cancel;
			}
			if ((interactionObj.Buttons & Interactions.MessageBoxButtons.Yes) != 0)
			{
				buttons |= Services.ModalDialog.Button.Yes;
			}
			if ((interactionObj.Buttons & Interactions.MessageBoxButtons.No) != 0)
			{
				buttons |= Services.ModalDialog.Button.No;
			}
			GameApp.Service<Services.ModalDialog>().Show(interactionObj.Text, buttons, btn =>
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
				interactionObj.Respond(ibtn);
			});
			return true;
		}
	}
}
