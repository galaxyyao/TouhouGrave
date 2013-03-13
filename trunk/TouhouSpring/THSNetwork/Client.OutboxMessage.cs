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
            }
        }

        private Dictionary<Interactions.BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>> m_outboxActionloopups
            = new Dictionary<BaseInteraction.PlayerAction, Action<Interactions.BaseInteraction, object>>();

        private void InitializeOutboxActionLookups()
        {
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.Pass, ProcessRespondPass);
            m_outboxActionloopups.Add(BaseInteraction.PlayerAction.Sacrifice, ProcessRespondSacrifice);
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
        #endregion

        public void ProcessRespond()
        {
            //Game.InteractionRespond respond = Game.RespondQueue.FirstOrDefault();
            //if (respond == null)
            //    throw new Exception("null respond");
            //if (respond.RespondType == Game.InteractionRespond.RespondEnum.TacticalPhase)
            //{
            //    Interactions.TacticalPhase.Result result = (Interactions.TacticalPhase.Result)respond.Result;
            //    switch (result.ActionType)
            //    {
            //        case Interactions.TacticalPhase.Action.Sacrifice:
            //            {
            //                SendMessage(string.Format("{0} sacrifice {1}", RoomId, respond.ResultSubjectIndex));
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else if (respond.RespondType == Game.InteractionRespond.RespondEnum.SelectCards)
            //{
            //}
            //else
            //{
            //    throw new Exception("Unhandled respond type");
            //}
            //    switch (Game.CurrentCommand.Type)
            //    {
            //        case Game.CurrentCommand.InteractionType.TacticalPhase:
            //            {
            //                Interactions.TacticalPhase.Result currentCommand = new Interactions.TacticalPhase.Result();
            //                if (Game.CurrentCommand.Result == null)
            //                    return true;
            //                currentCommand = (Interactions.TacticalPhase.Result)Game.CurrentCommand.Result;
            //                switch (currentCommand.ActionType)
            //                {
            //                    case Interactions.TacticalPhase.Action.Sacrifice:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} sacrifice {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.PlayCard:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} playcard {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.AttackCard:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} attackcard {1} {2}"
            //                                , _client.RoomId, Game.CurrentCommand.ResultSubjectIndex
            //                                , Game.CurrentCommand.ResultParameters[0]));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.AttackPlayer:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} attackplayer {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.ActivateAssist:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} activateassist {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.CastSpell:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} castspell {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    case Interactions.TacticalPhase.Action.Redeem:
            //                        {
            //                            _client.EnqueueMessage(string.Format("{0} redeem {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
            //                        }
            //                        break;
            //                    default:
            //                        break;
            //                }
            //            }
            //            break;
            //        case Game.CurrentCommand.InteractionType.SelectCards:
            //            {
            //                int[] selectedCardsIndexs = Game.CurrentCommand.ResultParameters;
            //                if (selectedCardsIndexs.Count() == 0)
            //                    return true;
            //                StringBuilder indexes = new StringBuilder();
            //                for (int i = 0; i < selectedCardsIndexs.Count(); i++)
            //                {
            //                    indexes.Append(selectedCardsIndexs[i]);
            //                    indexes.Append(" ");
            //                }
            //                indexes.Remove(indexes.Length - 1, 1);
            //                _client.EnqueueMessage(string.Format("{0} selectcards -1 {1}", _client.RoomId, indexes.ToString()));
            //            }
            //            break;
            //        case Game.CurrentCommand.InteractionType.Others:
            //            return true;
            //    }
            //    return true;
        }
    }
}
