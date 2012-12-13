using System;
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
            if (interactionObj.Card.Owner == Player)
            {
                switch (interactionObj.Notification)
                {
                    case "OnCardDestroyed":
                        GameApp.Service<GameUI>().UnregisterCard(interactionObj.Card);
                        break;
                    case "OnCardDrawn":
                        GameApp.Service<GameUI>().RegisterCard(interactionObj.Card);
                        break;
                    case "OnCardPlayCanceled":
                        Action action = () =>
                        {
                            if (GameApp.Service<GameManager>().Game.CurrentPhase == "Tactical")
                            {
                                GameApp.Service<GameUI>().TacticalPhase_Leave();
                            }
                            else if (GameApp.Service<GameManager>().Game.CurrentPhase == "Combat/Block")
                            {
                                GameApp.Service<GameUI>().BlockerPhase_ClearSelected();
                            }
                            GameApp.Service<GameUI>().InteractionObject = null;
                            interactionObj.Respond();
                        };

                        GameApp.Service<GameUI>().InteractionObject = interactionObj;
                        if (interactionObj.Message != string.Empty)
                        {
                            GameApp.Service<Services.ModalDialog>().Show(interactionObj.Message, action);
                        }
                        else
                        {
                            action();
                        }
                        return true;
                    case "OnCardPlayed":
                        GameApp.Service<GameUI>().RegisterCard(interactionObj.Card);
                        if (GameApp.Service<GameManager>().Game.CurrentPhase == "Tactical")
                        {
                            GameApp.Service<GameUI>().TacticalPhase_Leave();
                        }
                        else if (GameApp.Service<GameManager>().Game.CurrentPhase == "Combat/Block")
                        {
                            GameApp.Service<GameUI>().BlockerPhase_ClearSelected();
                        }
                        break;
                    case "OnCardSummoned":
                        GameApp.Service<GameUI>().RegisterCard(interactionObj.Card);
                        break;
                    default:
                        break;
                }
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

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyControllerEvent))]
		private bool OnNotified(Interactions.NotifyControllerEvent interactionObj)
		{
			switch (interactionObj.Notification)
			{
				case "OnPlayerPhaseChanged":
					break;
				case "OnPlayerDamaged":
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
            if (interactionObj.Spell.Host.Owner == Player)
            {
                switch (interactionObj.Notification)
                {
                    case "OnSpellCasted":
                        GameApp.Service<GameUI>().TacticalPhase_Leave();
                        break;
                    case "OnSpellCastCanceled":
                        GameApp.Service<Services.ModalDialog>().Show(interactionObj.Message, () =>
                        {
                            GameApp.Service<GameUI>().TacticalPhase_Leave();
                            GameApp.Service<GameUI>().InteractionObject = null;
                            interactionObj.Respond();
                        });
                        GameApp.Service<GameUI>().InteractionObject = interactionObj;
                        return true;
                    default:
                        break;
                }
            }

			interactionObj.Respond();
			return false;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.TacticalPhase))]
		private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
		{
			GameApp.Service<GameUI>().InteractionObject = interactionObj;
			GameApp.Service<GameUI>().TacticalPhase_Enter(interactionObj);
			return true;
		}

        [Interactions.MessageMap.Handler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            GameApp.Service<GameUI>().InteractionObject = interactionObj;
            GameApp.Service<GameUI>().SelectCards_Enter(interactionObj);
            return true;
        }

		[Interactions.MessageMap.Handler(typeof(Interactions.BlockPhase))]
		private bool OnDeclareBlockers(Interactions.BlockPhase interactionObj)
		{
			GameApp.Service<GameUI>().InteractionObject = interactionObj;
			GameApp.Service<GameUI>().BlockPhase_Enter(interactionObj);
			return true;
		}

		[Interactions.MessageMap.Handler(typeof(Interactions.MessageBox))]
		private bool OnMessageBox(Interactions.MessageBox interactionObj)
		{
			GameApp.Service<GameUI>().InteractionObject = interactionObj;

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
				GameApp.Service<GameUI>().InteractionObject = null;

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
