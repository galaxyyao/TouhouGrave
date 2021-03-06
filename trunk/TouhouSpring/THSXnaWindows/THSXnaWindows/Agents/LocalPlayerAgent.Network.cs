﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Network = TouhouSpring.Network;

namespace TouhouSpring.Agents
{
    partial class LocalPlayerAgent
    {
        private Network.Client m_NetworkClient = null;

        public LocalPlayerAgent(int pid) : base(pid)
        {
            var client = GameApp.Service<Services.Network>().THSClient;
            if (client.NetworkStatus == Network.Client.NetworkStatusEnum.Connected)
            {
                m_NetworkClient = client;
            }
        }

        public LocalPlayerAgent(int pid, int seed)
            : base(pid, seed)
        {
            var client = GameApp.Service<Services.Network>().THSClient;
            if (client.NetworkStatus == Network.Client.NetworkStatusEnum.Connected)
            {
                m_NetworkClient = client;
            }
        }

        public override void OnInitiativeCommandEnd()
        {
            if (m_NetworkClient != null)
            {
                // flush the response queue thru network interface
                m_NetworkClient.FlushOutboxQueue();
            }
        }

        public override void OnInitiativeCommandCanceled()
        {
            if (m_NetworkClient != null)
            {
                // clear the response queue
                m_NetworkClient.DiscardOutboxQueue();
            }
        }

        public override void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            base.OnRespondBack(io, result);

            if (m_NetworkClient == null)
            {
                return;
            }

            if (io is Interactions.TacticalPhase)
            {
                var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;

                // queue
                m_NetworkClient.LocalLeaveInteraction(tacticalPhaseResult.ActionType, io, result);

                // if the response is AttackCard, AttackPlayer, Pass
                // Flush Outbox message queue
                if (tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.AttackCard
                    || tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.AttackPlayer
                    || tacticalPhaseResult.ActionType == Interactions.BaseInteraction.PlayerAction.Pass
                    )
                    m_NetworkClient.FlushOutboxQueue();

            }
            else if (io is Interactions.SelectCards
                || io is Interactions.SelectNumber
                || io is Interactions.MessageBox)
            {
                // queue
                if (io is Interactions.SelectCards)
                {
                    var selectCardsResult = result as IIndexable<CardInstance>;
                    m_NetworkClient.LocalLeaveInteraction(Interactions.BaseInteraction.PlayerAction.SelectCards, io, result);
                }
                else if (io is Interactions.SelectNumber)
                {
                    var selectCardsResult = result as int?;
                    m_NetworkClient.LocalLeaveInteraction(Interactions.BaseInteraction.PlayerAction.SelectNumber, io, result);
                }
                else
                {
                    throw new NotImplementedException();
                }

                throw new NotImplementedException();
                //if (io.Game.RunningCommand.ExecutionPhase != Commands.CommandPhase.Condition)
                //{
                //    // means the command will never be canceled
                //    // flush
                //    m_NetworkClient.FlushOutboxQueue();
                //}
            }
        }
    }
}
