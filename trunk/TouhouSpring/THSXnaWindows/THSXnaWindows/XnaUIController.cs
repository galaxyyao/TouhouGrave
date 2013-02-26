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
        private Agents.BaseAgent[] m_agents;

        public IIndexable<Agents.BaseAgent> Agents
        {
            get; private set;
        }

        public XnaUIController(Agents.BaseAgent[] agents)
        {
            m_agents = agents;
            Agents = m_agents.ToIndexable();
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyOnly))]
        private bool OnNotified(Interactions.NotifyOnly interactionObj)
        {
            throw new InvalidOperationException("NotifyOnly shall not occur.");
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyCardEvent))]
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
                    if (m_agents[Game.Players.IndexOf(Game.ActingPlayer)].OnCardPlayCanceled(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyGameEvent))]
        private bool OnNotified(Interactions.NotifyGameEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyPlayerEvent))]
        private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnTurnEnded":
                    if (m_agents[Game.Players.IndexOf(Game.ActingPlayer)].OnTurnEnded(interactionObj))
                    {
                        return true;
                    }
                    break;
                case "OnTurnStarted":
                    if (m_agents[Game.Players.IndexOf(Game.ActingPlayer)].OnTurnStarted(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifySpellEvent))]
        private bool OnNotified(Interactions.NotifySpellEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnSpellCasted":
                    break;
                case "OnSpellCastCanceled":
                    if (m_agents[Game.Players.IndexOf(Game.ActingPlayer)].OnSpellCastCanceled(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.TacticalPhase))]
        private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            m_agents[Game.Players.IndexOf(interactionObj.Player)].OnTacticalPhase(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            m_agents[Game.Players.IndexOf(interactionObj.Player)].OnSelectCards(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            m_agents[Game.Players.IndexOf(interactionObj.Player)].OnMessageBox(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectNumber))]
        private bool OnSelectNumber(Interactions.SelectNumber interactionObj)
        {
            m_agents[Game.Players.IndexOf(interactionObj.Player)].OnSelectNumber(interactionObj);
            return true;
        }
    }
}
