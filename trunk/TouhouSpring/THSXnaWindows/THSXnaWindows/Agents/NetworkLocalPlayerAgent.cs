using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Network = TouhouSpring.Network;

namespace TouhouSpring.Agents
{
    class NetworkLocalPlayerAgent : LocalPlayerAgent
    {
        private Network.Client m_NetworkClient = null;

        public NetworkLocalPlayerAgent()
        {
            m_NetworkClient = GameApp.Service<Services.Network>().THSClient;
        }

        public override void OnInitiativeCommandEnd()
        {
            // flush the response queue thru network interface
            m_NetworkClient.OutboxQueue.Flush();
        }

        public override void OnInitiativeCommandCanceled()
        {
            // clear the response queue
            m_NetworkClient.OutboxQueue.Clear();
        }

        public override void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            if (io is Interactions.TacticalPhase)
            {
                var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;

                // queue
                m_NetworkClient.ProcessRespond(tacticalPhaseResult.ActionType, io, result);

                // if the response is AttackCard, AttackPlayer, Pass
                // Flush Outbox message queue
                if (tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.AttackCard
                    || tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.AttackPlayer
                    || tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.Pass
                    )
                    m_NetworkClient.OutboxQueue.Flush();

            }
            else if (io is Interactions.SelectCards
                || io is Interactions.SelectNumber
                || io is Interactions.MessageBox)
            {
                // queue
                if (io is Interactions.SelectCards)
                {
                    var selectCardsResult = (IIndexable<CardInstance>)result;
                    m_NetworkClient.ProcessRespond(Interactions.BaseInteraction.PlayerAction.SelectCards, io, result);
                }

                if (io is Interactions.SelectNumber)
                {
                    var selectCardsResult = (int?)result;
                    m_NetworkClient.ProcessRespond(Interactions.BaseInteraction.PlayerAction.SelectNumber, io, result);
                }

                if (io.Game.RunningCommand.ExecutionPhase != Commands.CommandPhase.Condition)
                {
                    // means the command will never be canceled
                    // flush
                    m_NetworkClient.OutboxQueue.Flush();
                }
            }
        }
    }
}
