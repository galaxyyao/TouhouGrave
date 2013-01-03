using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    class ConsoleController : BaseController
    {
        [Interactions.MessageMap.Handler(typeof(Interactions.NotifyOnly))]
        private bool OnNotified(Interactions.NotifyOnly interactionObj)
        {
			throw new ArgumentException("Interactions.NotifyOnly is no longer in use.");
        }

        [Interactions.MessageMap.Handler(typeof(Interactions.NotifyCardEvent))]
        private bool OnNotified(Interactions.NotifyCardEvent interactionObj)
        {
            if (interactionObj.Card.Owner == Player)
            {
                switch (interactionObj.Notification)
                {
                    case "OnCardDrawn":
                        Console.WriteLine(">> Player {0} drew card {1}", interactionObj.Card.Owner.Name, interactionObj.Card.Model.Name);
                        break;
                    case "OnCardPlayed":
                        Console.WriteLine(">> Player {0} played card {1} onto the battlefield.", interactionObj.Card.Owner.Name, interactionObj.Card.Model.Name);
                        break;
                    case "OnCardPlayCanceled":
                        Console.WriteLine(">> Player {0} canceled playing card {1}: {2}", interactionObj.Card.Owner.Name, interactionObj.Card.Model.Name, interactionObj.Message);
                        break;
                    case "OnCardDestroyed":
                        Console.WriteLine(">> Player {0} card {1} is Destroyed", interactionObj.Card.Owner.Name, interactionObj.Card.Model.Name);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Argument {0} is illegal.", interactionObj.Notification));
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

		[Interactions.MessageMap.Handler(typeof(Interactions.NotifyPlayerEvent))]
		private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
		{
            if (interactionObj.Player == Player)
            {
                switch (interactionObj.Notification)
                {
                    case "OnPlayerPhaseChanged":
                        {
                            Console.WriteLine(">> ---Player {0}'s {1}---", interactionObj.Player.Name, interactionObj.Message);
                        }
                        break;
                    case "OnPlayerDamaged":
                        Console.WriteLine(">> Player {0} suffers damage {1}.", interactionObj.Player.Name, interactionObj.Message);
                        break;
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
            Debug.Assert(Program.ActiveInteraction == null);
            Program.ActiveInteraction = interactionObj;
            return true;
        }

        [Interactions.MessageMap.Handler(typeof(Interactions.SelectCards))]
        private bool OnSelectCard(Interactions.SelectCards interactionObj)
        {
            Console.WriteLine(interactionObj.Message);
            Debug.Assert(Program.ActiveInteraction == null);
            Program.ActiveInteraction = interactionObj;
            return true;
        }
    }
}
