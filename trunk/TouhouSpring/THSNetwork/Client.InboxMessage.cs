using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Diagnostics;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        public class InboxMessageQueue : MessageQueue
        {
            public InboxMessageQueue(Client client)
                : base(client)
            {
            }

            public override void Flush()
            {
                foreach (var message in m_messageQueue)
                {
                    m_client.IntepretMessage(message);
                }
                Clear();
            }
        }

        private Dictionary<string, Action<List<string>>> m_inboxActionloopups = new Dictionary<string, Action<List<string>>>();

        private void InitializeInboxActionLookups()
        {
            m_inboxActionloopups.Add("enterroom", IntepretMessageEnterRoom);
            m_inboxActionloopups.Add("waiting", IntepretMessageWaiting);
            m_inboxActionloopups.Add("disconnect", IntepretMessageDisconnect);
            m_inboxActionloopups.Add("startgame", IntepretMessageStartGame);
            m_inboxActionloopups.Add("generateseed", IntepretMessageGenerateSeed);
            m_inboxActionloopups.Add("switchturn", IntepretMessageSwitchTurn);
            m_inboxActionloopups.Add("sacrifice", IntepretMessageSacrifice);
            m_inboxActionloopups.Add("playcard", IntepretMessagePlayCard);
            m_inboxActionloopups.Add("attackcard", IntepretMessageAttackCard);
            m_inboxActionloopups.Add("attackplayer", IntepretMessageAttackPlayer);
            m_inboxActionloopups.Add("activateassist", IntepretMessageActivateAssist);
            m_inboxActionloopups.Add("castspell", IntepretMessageCastSpell);
            m_inboxActionloopups.Add("redeem", IntepretMessageRedeem);
            m_inboxActionloopups.Add("selectcards", IntepretMessageSelectCards);
        }

        public void IntepretMessage(string message)
        {
            List<string> parts = message.Split(' ').ToList();
            Action<List<string>> intepretMessageAction;
            if (m_inboxActionloopups.TryGetValue(parts[1], out intepretMessageAction))
            {
                intepretMessageAction.Invoke(parts);
            }
            else
            {
                throw new NotImplementedException("The method for {0} has not been implemented.");
            }
        }

        #region Message Intepret List
        private void IntepretMessageEnterRoom(List<string> parts)
        {
            RoomId = Convert.ToInt32(parts[0]);
            Seed = -1;
            SendMessage(string.Format("{0} {1}", RoomId, "roomentered"));
        }

        private void IntepretMessageWaiting(List<string> parts)
        {

        }

        private void IntepretMessageDisconnect(List<string> parts)
        {

        }

        private void IntepretMessageStartGame(List<string> parts)
        {
            RoomStatus = RoomStatusEnum.Starting;
            StartupIndex = Convert.ToInt32(parts[2]);
            SendMessage(string.Format("{0} {1}", RoomId, "gamestarted"));
        }

        private void IntepretMessageGenerateSeed(List<string> parts)
        {
            Seed = Convert.ToInt32(parts[2]);
        }

        private void IntepretMessageSwitchTurn(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).RespondPass();
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageSacrifice(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondSacrifice(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).SacrificeCandidates[Convert.ToInt32(parts[2])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessagePlayCard(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondPlay(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).PlayCardCandidates[Convert.ToInt32(parts[2])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageAttackCard(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondAttackCard(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates[Convert.ToInt32(parts[2])]
                    , ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).DefenderCandidates[Convert.ToInt32(parts[3])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageAttackPlayer(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondAttackPlayer(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates[Convert.ToInt32(parts[2])]
                    , CurrentGame.CurrentInteraction.Game.ActingPlayerEnemies.FirstOrDefault());
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageActivateAssist(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondActivate(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).ActivateAssistCandidates[Convert.ToInt32(parts[2])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageCastSpell(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondCast(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).CastSpellCandidates[Convert.ToInt32(parts[2])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageRedeem(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.TacticalPhase))
                    throw new Exception("Wrong Phase");
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondRedeem(((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).RedeemCandidates[Convert.ToInt32(parts[2])]);
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void IntepretMessageSelectCards(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                if (!(CurrentGame.CurrentInteraction is Interactions.SelectCards))
                    throw new Exception("Wrong Phase");
                List<CardInstance> selectedCards = new List<CardInstance>();
                for (int i = 3; i < parts.Count; i++)
                {
                    selectedCards.Add(((Interactions.SelectCards)CurrentGame.CurrentInteraction).Candidates[Convert.ToInt32(parts[i])]);
                }
                ((Interactions.SelectCards)CurrentGame.CurrentInteraction)
                    .Respond(selectedCards.ToIndexable());
                CurrentGame.CurrentInteraction = null;
            }
        }
        #endregion


        //case "selectcards":
        //    {
        //        if (CurrentIo == null)
        //        {
        //            RemoteCommand command = new RemoteCommand
        //            {
        //                RemoteAction = RemoteCommand.RemoteActionEnum.SelectCards,
        //            };
        //            List<int> parameters = new List<int>();
        //            for (int i = 3; i < parts.Count; i++)
        //            {
        //                parameters.Add(Convert.ToInt32(parts[i]));
        //            }
        //            command.ResultParameters = parameters.ToArray();
        //            RemoteCommandEnqueue(command);
        //            List<CardInstance> selectedCards = new List<CardInstance>();
        //            for (int i = 3; i < parts.Count; i++)
        //            {
        //                selectedCards.Add(((Interactions.SelectCards)CurrentIo).Candidates[Convert.ToInt32(parts[i])]);
        //            }
        //        }
        //        else
        //        {
        //            if (!(CurrentIo is Interactions.SelectCards))
        //                throw new Exception("Wrong Phase");
        //            List<CardInstance> selectedCards = new List<CardInstance>();
        //            for (int i = 3; i < parts.Count; i++)
        //            {
        //                selectedCards.Add(((Interactions.SelectCards)CurrentIo).Candidates[Convert.ToInt32(parts[i])]);
        //            }
        //            ((Interactions.SelectCards)CurrentIo)
        //                .Respond(selectedCards.ToIndexable());
        //            CurrentIo = null;
        //        }
        //    }
        //    break;
        //case "redeem":
        //    {
        //        if (CurrentIo == null)
        //            throw new Exception("current io is null");
        //        if (!(CurrentIo is Interactions.TacticalPhase))
        //            throw new Exception("Wrong Phase");
        //        ((Interactions.TacticalPhase)CurrentIo)
        //            .RespondRedeem(((Interactions.TacticalPhase)CurrentIo).RedeemCandidates[Convert.ToInt32(parts[2])]);
        //        CurrentIo = null;
        //    }
        //    break;
        //    default:
        //        Debug.Print("Invalid argument");
        //        break;
        //}
    }
}
