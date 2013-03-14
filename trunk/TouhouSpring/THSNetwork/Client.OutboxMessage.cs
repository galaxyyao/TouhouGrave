using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        public class OutboxMessageQueue : MessageQueue
        {
            public OutboxMessageQueue(Client client)
                : base(client)
            {
            }

            public override void Flush()
            {
                foreach (var message in m_messageQueue)
                {
                    SendMessage(message);
                }
                Clear();
            }

            public void SendMessage(string message)
            {
                m_client.SendMessage(message);
                System.Diagnostics.Debug.Print(string.Format("{0} Sent:{1}", m_client._client.UniqueIdentifier, message));
            }
        }

        private Dictionary<Interactions.BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>> m_outboxActionloopups
            = new Dictionary<BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>>();

        private void InitializeOutboxActionLookups()
        {
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.Pass, ProcessRespondPass);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.Sacrifice, ProcessRespondSacrifice);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.PlayCard, ProcessRespondPlayCard);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.AttackCard, ProcessRespondAttackCard);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.AttackPlayer, ProcessRespondAttackPlayer);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.ActivateAssist, ProcessRespondActivateAssist);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.CastSpell, ProcessRespondCastSpell);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.Redeem, ProcessRespondRedeem);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.SelectCards, ProcessSelectCards);
        }

        public void ProcessRespond(Interactions.BaseInteraction.PlayerAction action, Interactions.BaseInteraction io, object result)
        {
            Action<Interactions.BaseInteraction, object> processRespondAction;
            if (m_outboxActionloopups.TryGetValue(action, out processRespondAction))
            {
                processRespondAction.Invoke(io, result);
            }
            else
            {
                throw new NotImplementedException("The method for {0} has not been implemented.");
            }
        }

        #region Process Respond List
        private void ProcessRespondPass(Interactions.BaseInteraction io, object result)
        {
            OutboxQueue.Queue(string.Format("{0} switchturn", RoomId));
        }

        private void ProcessRespondSacrifice(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.SacrificeCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            OutboxQueue.Queue(string.Format("{0} sacrifice {1}", RoomId, index));
        }

        private void ProcessRespondPlayCard(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.PlayCardCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            OutboxQueue.Queue(string.Format("{0} playcard {1}", RoomId, index));
        }

        private void ProcessRespondAttackCard(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var attackCardObjs = (CardInstance[])tacticalPhaseResult.Data;
            var attackerIndex = tacticalPhaseIo.AttackerCandidates.IndexOf(attackCardObjs[0]);
            var defenserIndex = tacticalPhaseIo.DefenderCandidates.IndexOf(attackCardObjs[1]);
            OutboxQueue.Queue(string.Format("{0} attackcard {1} {2}", RoomId, attackerIndex, defenserIndex));
        }

        private void ProcessRespondAttackPlayer(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var attackPlayerObjs = (object[])tacticalPhaseResult.Data;
            var attackerIndex = tacticalPhaseIo.AttackerCandidates.IndexOf((CardInstance)attackPlayerObjs[0]);
            OutboxQueue.Queue(string.Format("{0} attackplayer {1}", RoomId, attackerIndex));
        }

        private void ProcessRespondActivateAssist(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.ActivateAssistCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            OutboxQueue.Queue(string.Format("{0} activateassist {1}", RoomId, index));
        }

        private void ProcessRespondCastSpell(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.CastSpellCandidates.IndexOf((Behaviors.ICastableSpell)tacticalPhaseResult.Data);
            OutboxQueue.Queue(string.Format("{0} castspell {1}", RoomId, index));
        }

        private void ProcessRespondRedeem(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.RedeemCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            OutboxQueue.Queue(string.Format("{0} redeem {1}", RoomId, index));
        }

        private void ProcessSelectCards(Interactions.BaseInteraction io, object result)
        {
            var selectCardsIo = (Interactions.SelectCards)io;
            var selectCardsResult = (IIndexable<CardInstance>)result;
            List<int> indexes = new List<int>();
            foreach (CardInstance selectedCard in selectCardsResult)
            {
                indexes.Add(selectCardsIo.Candidates.IndexOf(selectedCard));
            }
            OutboxQueue.Queue(string.Format("{0} selectcards -1 {1}", RoomId, string.Join(" ", indexes)));
        }

        #endregion
    }
}
