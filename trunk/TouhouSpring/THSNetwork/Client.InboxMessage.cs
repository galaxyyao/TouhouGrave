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
                    m_client.InterpretMessage(message);
                }
                Clear();
            }
        }

        private Dictionary<string, Action<List<string>>> m_inboxActionloopups = new Dictionary<string, Action<List<string>>>();

        private void InitializeInboxActionLookups()
        {
            m_inboxActionloopups.Add("enterroom", InterpretMessageEnterRoom);
            m_inboxActionloopups.Add("waiting", InterpretMessageWaiting);
            m_inboxActionloopups.Add("disconnect", InterpretMessageDisconnect);
            m_inboxActionloopups.Add("startgame", InterpretMessageStartGame);
            m_inboxActionloopups.Add("generateseed", InterpretMessageGenerateSeed);
            m_inboxActionloopups.Add("switchturn", InterpretMessageSwitchTurn);
            m_inboxActionloopups.Add("sacrifice", InterpretMessageSacrifice);
            m_inboxActionloopups.Add("playcard", InterpretMessagePlayCard);
            m_inboxActionloopups.Add("attackcard", InterpretMessageAttackCard);
            m_inboxActionloopups.Add("attackplayer", InterpretMessageAttackPlayer);
            m_inboxActionloopups.Add("activateassist", InterpretMessageActivateAssist);
            m_inboxActionloopups.Add("castspell", InterpretMessageCastSpell);
            m_inboxActionloopups.Add("redeem", InterpretMessageRedeem);
            m_inboxActionloopups.Add("selectcards", InterpretMessageSelectCards);
            m_inboxActionloopups.Add("selectnumber", InterpretMessageSelectNumber);
        }

        public void InterpretMessage(string message)
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

        #region Message Interpret List
        private void InterpretMessageEnterRoom(List<string> parts)
        {
            RoomId = Convert.ToInt32(parts[0]);
            Seed = -1;
            SendMessage(string.Format("{0} {1}", RoomId, "roomentered"));
        }

        private void InterpretMessageWaiting(List<string> parts)
        {

        }

        private void InterpretMessageDisconnect(List<string> parts)
        {

        }

        private void InterpretMessageStartGame(List<string> parts)
        {
            RoomStatus = RoomStatusEnum.Starting;
            StartupIndex = Convert.ToInt32(parts[2]);
            SendMessage(string.Format("{0} {1}", RoomId, "gamestarted"));
        }

        private void InterpretMessageGenerateSeed(List<string> parts)
        {
            Seed = Convert.ToInt32(parts[2]);
        }

        private void InterpretMessageSwitchTurn(List<string> parts)
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

        private void InterpretMessageSacrifice(List<string> parts)
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
                Debug.Print(string.Format("SelectCardsCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).SacrificeCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessagePlayCard(List<string> parts)
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
                Debug.Print(string.Format("PlayCardsCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).PlayCardCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageAttackCard(List<string> parts)
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
                Debug.Print(string.Format("AttackerCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates.Select(candidate => candidate.Guid.ToString()))));
                Debug.Print(string.Format("DefenderCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).DefenderCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageAttackPlayer(List<string> parts)
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
                Debug.Print(string.Format("AttackerCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).AttackerCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageActivateAssist(List<string> parts)
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
                Debug.Print(string.Format("AssistCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).ActivateAssistCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageCastSpell(List<string> parts)
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
                Debug.Print(string.Format("SpellCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).CastSpellCandidates.Select(candidate => candidate.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageRedeem(List<string> parts)
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
                Debug.Print(string.Format("RedeemCandidates:{0}"
                    , string.Join(",", ((Interactions.TacticalPhase)CurrentGame.CurrentInteraction).RedeemCandidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageSelectCards(List<string> parts)
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
                Debug.Print(string.Format("SelectCandidates:{0}"
                    , string.Join(",", ((Interactions.SelectCards)CurrentGame.CurrentInteraction).Candidates.Select(candidate => candidate.Guid.ToString()))));
                CurrentGame.CurrentInteraction = null;
            }
        }

        private void InterpretMessageSelectNumber(List<string> parts)
        {
            if (CurrentGame.CurrentInteraction == null)
            {
                InboxQueue.Queue(string.Join(" ", parts));
            }
            else
            {
                var selectNumber = CurrentGame.CurrentInteraction as Interactions.SelectNumber;
                if (selectNumber == null)
                {
                    throw new Exception("Wrong Phase");
                }

                if (parts[2] == "null")
                {
                    selectNumber.Respond(null);
                }
                else
                {
                    selectNumber.Respond(Int32.Parse(parts[2]));
                }
                CurrentGame.CurrentInteraction = null;
            }
        }
        #endregion
    }
}
