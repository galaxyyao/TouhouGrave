using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private List<string> _localMessageQueue = new List<string>();

        public void QueueLocalMessage(string message)
        {
            _localMessageQueue.Add(string.Format("{0} {1}", RoomId, message));
        }

        public void FlushLocalMessage()
        {
            foreach (var message in _localMessageQueue)
            {
                SendMessage(message);
            }
        }

        public void ClearLocalMessage()
        {
            _localMessageQueue.Clear();
        }

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
