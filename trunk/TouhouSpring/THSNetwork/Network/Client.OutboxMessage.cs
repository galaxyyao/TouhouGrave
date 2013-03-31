using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private Dictionary<Interactions.BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>> m_outboxActionloopups
            = new Dictionary<Interactions.BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>>();

        private Queue<string> m_outboxQueue = new Queue<string>();

        public void LocalLeaveInteraction(Interactions.BaseInteraction.PlayerAction action, Interactions.BaseInteraction io, object result)
        {
            Action<Interactions.BaseInteraction, object> processRespondAction;
            if (m_outboxActionloopups.TryGetValue(action, out processRespondAction))
            {
                processRespondAction(io, result);
            }
            else
            {
                throw new NotImplementedException("The method for {0} has not been implemented.");
            }
        }

        public void DiscardOutboxQueue()
        {
            m_outboxQueue.Clear();
        }

        public void FlushOutboxQueue()
        {
            while (m_outboxQueue.Count != 0)
            {
                SendMessage(m_outboxQueue.Dequeue());
            }
        }

        private void InitializeOutboxActionTable()
        {
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.Pass, ProcessRespondPass);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.Sacrifice, ProcessRespondSacrifice);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.PlayCard, ProcessRespondPlayCard);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.AttackCard, ProcessRespondAttackCard);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.AttackPlayer, ProcessRespondAttackPlayer);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.ActivateAssist, ProcessRespondActivateAssist);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.CastSpell, ProcessRespondCastSpell);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.Redeem, ProcessRespondRedeem);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.SelectCards, ProcessSelectCards);
            m_outboxActionloopups.Add(Interactions.BaseInteraction.PlayerAction.SelectNumber, ProcessSelectNumber);
        }

        private void EnqueueOutboxMessage(string format, params object[] args)
        {
            var msg = String.Format(CultureInfo.InvariantCulture, format, args).Trim();
            m_outboxQueue.Enqueue(msg);
        }

        #region Process Respond List

        private void ProcessRespondPass(Interactions.BaseInteraction io, object result)
        {
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{0}</Time><Game><Action>SwitchTurn</Action></Game></Message>"
                , DateTime.Now);
        }

        private void ProcessRespondSacrifice(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.SacrificeCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{1}</Time><Game><Action>Sacrifice</Action><SacrificeIndex>{0}</SacrificeIndex></Game></Message>"
                , index
                , DateTime.Now);
        }

        private void ProcessRespondPlayCard(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.PlayCardCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{1}</Time><Game><Action>PlayCard</Action><PlayCardIndex>{0}</PlayCardIndex></Game></Message>"
                , index
                , DateTime.Now);
        }

        private void ProcessRespondAttackCard(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var attackCardObjs = (CardInstance[])tacticalPhaseResult.Data;
            var attackerIndex = tacticalPhaseIo.AttackerCandidates.IndexOf(attackCardObjs[0]);
            var defenserIndex = tacticalPhaseIo.DefenderCandidates.IndexOf(attackCardObjs[1]);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{2}</Time><Game><Action>AttackCard</Action><AttackerIndex>{0}</AttackerIndex><DefenderIndex>{1}</DefenderIndex></Game></Message>"
                , attackerIndex
                , defenserIndex
                , DateTime.Now);
        }

        private void ProcessRespondAttackPlayer(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var attackPlayerObjs = (object[])tacticalPhaseResult.Data;
            var attackerIndex = tacticalPhaseIo.AttackerCandidates.IndexOf((CardInstance)attackPlayerObjs[0]);
            var playerIndex = ((Player)attackPlayerObjs[1]).Index;
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{2}</Time><Game><Action>AttackCard</Action><AttackerIndex>{0}</AttackerIndex><PlayerIndex>{1}</PlayerIndex></Game></Message>"
                , attackerIndex
                , playerIndex
                , DateTime.Now);
        }

        private void ProcessRespondActivateAssist(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.ActivateAssistCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{1}</Time><Game><Action>ActivateAssist</Action><AssistIndex>{0}</AssistIndex></Game></Message>"
                , index
                , DateTime.Now);
        }

        private void ProcessRespondCastSpell(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.CastSpellCandidates.IndexOf((Behaviors.ICastableSpell)tacticalPhaseResult.Data);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time{1}</Time><Game><Action>CastSpell</Action><CastSpellIndex>{0}</CastSpellIndex></Game></Message>"
                , index
                , DateTime.Now);
        }

        private void ProcessRespondRedeem(Interactions.BaseInteraction io, object result)
        {
            var tacticalPhaseIo = (Interactions.TacticalPhase)io;
            var tacticalPhaseResult = (Interactions.TacticalPhase.Result)result;
            var index = tacticalPhaseIo.RedeemCandidates.IndexOf((CardInstance)tacticalPhaseResult.Data);
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{1}</Time><Game><Action>Redeem</Action><RedeemIndex>{0}</RedeemIndex></Game></Message>"
                , index
                , DateTime.Now);
        }

        private void ProcessSelectCards(Interactions.BaseInteraction io, object result)
        {
            var selectCardsIo = (Interactions.SelectCards)io;
            var selectCardsResult = (IIndexable<CardInstance>)result;
            StringBuilder message = new StringBuilder();
            message.Append("<Message><Type>Game</Type><Time>");
            message.Append(DateTime.Now);
            message.Append("</Time><Game><Action>SelectCards</Action><SelectCards>");
            foreach (CardInstance selectedCard in selectCardsResult)
            {
                message.Append("<Index>");
                message.Append(selectCardsIo.Candidates.IndexOf(selectedCard));
                message.Append("</Index>");
            }
            message.Append("</SelectCards></Game></Message>");
            EnqueueOutboxMessage(message.ToString());
        }

        private void ProcessSelectNumber(Interactions.BaseInteraction io, object result)
        {
            var selectNumberIo = (Interactions.SelectNumber)io;
            var selectNumberResult = (int?)result;
            EnqueueOutboxMessage(
                "<Message><Type>Game</Type><Time>{1}</Time><Game><Action>SelectNumber</Action><Number>{0}</Number></Game></Message>"
                , selectNumberResult == null ? string.Empty : selectNumberResult.Value.ToString()
                , DateTime.Now);
        }

        #endregion
    }
}
