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
                var sacrificedCard = ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).SacrificeCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondSacrifice(sacrificedCard);
                Debug.Print(string.Format("Sacrificed {0}", sacrificedCard.Guid));
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
                var playedCard = ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).PlayCardCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondPlay(playedCard);
                Debug.Print(string.Format("Played {0}", playedCard.Guid));
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
                var attackerCard=((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates[Convert.ToInt32(parts[2])];
                var defenderCard=((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).DefenderCandidates[Convert.ToInt32(parts[3])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondAttackCard(attackerCard, defenderCard);
                Debug.Print(string.Format("{0} attacked {1}", attackerCard.Guid, defenderCard.Guid));
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
                var attackerCard=((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondAttackPlayer(attackerCard
                    , CurrentGame.CurrentInteraction.Game.ActingPlayerEnemies.FirstOrDefault());
                Debug.Print(string.Format("{0} attacked player", attackerCard.Guid));
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
                var assistCard = ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).ActivateAssistCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondActivate(assistCard);
                Debug.Print(string.Format("Activated {0}", assistCard.Guid));
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
                var spell = ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).CastSpellCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondCast(spell);
                Debug.Print(string.Format("Casted {0}", spell.ToString()));
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
                var redeemedCard = ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).RedeemCandidates[Convert.ToInt32(parts[2])];
                ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction)
                    .RespondRedeem(redeemedCard);
                Debug.Print(string.Format("Redeemed {0}", redeemedCard.Guid));
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
                    var selectedCard=((Interactions.SelectCards)CurrentGame.CurrentInteraction).Candidates[Convert.ToInt32(parts[i])];
                    selectedCards.Add(selectedCard);
                    Debug.Print(string.Format("Selected {0}", selectedCard.Guid));
                }
                ((Interactions.SelectCards)CurrentGame.CurrentInteraction)
                    .Respond(selectedCards.ToIndexable());
                CurrentGame.CurrentInteraction = null;
            }
        }
        #endregion
    }
}
