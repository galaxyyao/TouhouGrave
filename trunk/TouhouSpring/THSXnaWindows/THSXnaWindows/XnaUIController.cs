using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Services;
using TouhouSpring.UI;
using THSNetwork = TouhouSpring.Network;

namespace TouhouSpring
{
    class XnaUIController : BaseController
    {
        private Agents.BaseAgent[] m_agents;
        private List<int> m_destroyedCards = new List<int>();

        public int RandomSeed
        {
            get; private set;
        }

        public IIndexable<Agents.BaseAgent> Agents
        {
            get; private set;
        }

        public IEnumerable<int> DestroyedCards
        {
            get { return m_destroyedCards; }
        }

        public XnaUIController(Agents.BaseAgent[] agents, int seed)
        {
            m_agents = agents;
            Agents = m_agents.ToIndexable();
            RandomSeed = seed;
        }

        public override int GetRandomSeed()
        {
            return RandomSeed;
        }

        protected override bool OnNotified(Interactions.NotifyCardEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnCardDestroyed":
                    m_destroyedCards.Add(interactionObj.Card.Guid);
                    break;
                case "OnCardPlayCanceled":
                    if (m_agents[Game.ActingPlayer.Index].OnCardPlayCanceled(interactionObj))
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

        protected override bool OnNotified(Interactions.NotifyGameEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnInitiativeCommandEnd":
                    m_agents[Game.ActingPlayer.Index].OnInitiativeCommandEnd();
                    break;
                case "OnInitiativeCommandCanceled":
                    m_agents[Game.ActingPlayer.Index].OnInitiativeCommandCanceled();
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        protected override bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnTurnEnded":
                    if (m_agents[Game.ActingPlayer.Index].OnTurnEnded(interactionObj))
                    {
                        return true;
                    }
                    break;
                case "OnTurnStarted":
                    if (m_agents[Game.ActingPlayer.Index].OnTurnStarted(interactionObj))
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

        protected override bool OnNotified(Interactions.NotifySpellEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnSpellCasted":
                    break;
                case "OnSpellCastCanceled":
                    if (m_agents[Game.ActingPlayer.Index].OnSpellCastCanceled(interactionObj))
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

        protected override bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            m_agents[interactionObj.Player.Index].OnTacticalPhase(interactionObj);
            return true;
        }

        protected override bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            m_agents[interactionObj.Player.Index].OnSelectCards(interactionObj);
            return true;
        }

        protected override bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            m_agents[interactionObj.Player.Index].OnMessageBox(interactionObj);
            return true;
        }

        protected override bool OnSelectNumber(Interactions.SelectNumber interactionObj)
        {
            m_agents[interactionObj.Player.Index].OnSelectNumber(interactionObj);
            return true;
        }

        public override void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            if (io is Interactions.TacticalPhase)
            {
                m_agents[(io as Interactions.TacticalPhase).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.SelectCards)
            {
                m_agents[(io as Interactions.SelectCards).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.SelectNumber)
            {
                m_agents[(io as Interactions.SelectNumber).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.NotifyOnly
                || io is Interactions.NotifyCardEvent
                || io is Interactions.NotifyPlayerEvent
                || io is Interactions.NotifyGameEvent
                || io is Interactions.NotifySpellEvent
                )
            {
                //let notifyonly be
            }
            else
            {
                throw new Exception("Unhandled Response back Interation type.");
            }
        }

        public override void OnCommandFlushed()
        {
            GameApp.Service<GameManager>().RefreshGameEvaluators();
        }
    }
}
