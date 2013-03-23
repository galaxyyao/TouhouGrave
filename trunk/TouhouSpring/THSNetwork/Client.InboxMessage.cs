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
        private Dictionary<string, Action<string[]>> m_interactionActions = new Dictionary<string, Action<string[]>>();

        private Interactions.BaseInteraction m_remoteInteraction;
        private Queue<string[]> m_interactionMessageQueue = new Queue<string[]>();

        public void RemoteEnterInteraction(Interactions.BaseInteraction io)
        {
            Debug.Assert(m_remoteInteraction == null);
            m_remoteInteraction = io;
            if (m_interactionMessageQueue.Count != 0)
            {
                var msg = m_interactionMessageQueue.Dequeue();
                ProcessInteractionMessage(msg);
            }
        }

        private void InitializeInteractionActionsTable()
        {
            m_interactionActions.Add("switchturn", InterpretMessageSwitchTurn);
            m_interactionActions.Add("sacrifice", InterpretMessageSacrifice);
            m_interactionActions.Add("playcard", InterpretMessagePlayCard);
            m_interactionActions.Add("attackcard", InterpretMessageAttackCard);
            m_interactionActions.Add("attackplayer", InterpretMessageAttackPlayer);
            m_interactionActions.Add("activateassist", InterpretMessageActivateAssist);
            m_interactionActions.Add("castspell", InterpretMessageCastSpell);
            m_interactionActions.Add("redeem", InterpretMessageRedeem);
            m_interactionActions.Add("selectcards", InterpretMessageSelectCards);
            m_interactionActions.Add("selectnumber", InterpretMessageSelectNumber);
        }

        private void OnInteractionMessageArrive(string[] messageParts)
        {
            if (m_remoteInteraction == null)
            {
                m_interactionMessageQueue.Enqueue(messageParts);
            }
            else
            {
                ProcessInteractionMessage(messageParts);
            }
        }

        private void ProcessInteractionMessage(string[] parts)
        {
            Action<string[]> action;
            if (m_interactionActions.TryGetValue(parts[1], out action))
            {
                action(parts);
            }
            else
            {
                throw new NotImplementedException("The method for {0} has not been implemented.");
            }
        }

        #region Interaction message actions

        private void InterpretMessageSwitchTurn(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            tacticalPhase.RespondPass();
            m_remoteInteraction = null;
        }

        private void InterpretMessageSacrifice(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var sacrificedCard = tacticalPhase.SacrificeCandidates[Convert.ToInt32(parts[2])];
            tacticalPhase.RespondSacrifice(sacrificedCard);
            Debug.Print(string.Format("Sacrificed {0}", sacrificedCard.Guid));
            Debug.Print(string.Format("SelectCardsCandidates:{0}"
                , string.Join(",", tacticalPhase.SacrificeCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessagePlayCard(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var playedCard = tacticalPhase.PlayCardCandidates[Convert.ToInt32(parts[2])];
            tacticalPhase.RespondPlay(playedCard);
            Debug.Print(string.Format("Played {0}", playedCard.Guid));
            Debug.Print(string.Format("PlayCardsCandidates:{0}"
                , string.Join(",", tacticalPhase.PlayCardCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageAttackCard(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var attackerCard = tacticalPhase.AttackerCandidates[Convert.ToInt32(parts[2])];
            var defenderCard = tacticalPhase.DefenderCandidates[Convert.ToInt32(parts[3])];
            tacticalPhase.RespondAttackCard(attackerCard, defenderCard);
            Debug.Print(string.Format("{0} attacked {1}", attackerCard.Guid, defenderCard.Guid));
            Debug.Print(string.Format("AttackerCandidates:{0}"
                , string.Join(",", tacticalPhase.AttackerCandidates.Select(candidate => candidate.Guid.ToString()))));
            Debug.Print(string.Format("DefenderCandidates:{0}"
                , string.Join(",", tacticalPhase.DefenderCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageAttackPlayer(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var attackerCard = tacticalPhase.AttackerCandidates[Convert.ToInt32(parts[2])];
            var playerBeingAttacked = tacticalPhase.Game.Players[Convert.ToInt32(parts[3])];
            tacticalPhase.RespondAttackPlayer(attackerCard, playerBeingAttacked);
            Debug.Print(string.Format("{0} attacked player {1}", attackerCard.Guid, playerBeingAttacked.Name));
            Debug.Print(string.Format("AttackerCandidates:{0}"
                , string.Join(",", tacticalPhase.AttackerCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageActivateAssist(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var assistCard = tacticalPhase.ActivateAssistCandidates[Convert.ToInt32(parts[2])];
            tacticalPhase.RespondActivate(assistCard);
            Debug.Print(string.Format("Activated {0}", assistCard.Guid));
            Debug.Print(string.Format("AssistCandidates:{0}"
                , string.Join(",", tacticalPhase.ActivateAssistCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageCastSpell(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var spell = tacticalPhase.CastSpellCandidates[Convert.ToInt32(parts[2])];
            tacticalPhase.RespondCast(spell);
            Debug.Print(string.Format("Casted {0}", spell.ToString()));
            Debug.Print(string.Format("SpellCandidates:{0}"
                , string.Join(",", tacticalPhase.CastSpellCandidates.Select(candidate => candidate.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageRedeem(string[] parts)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var redeemedCard = tacticalPhase.RedeemCandidates[Convert.ToInt32(parts[2])];
            tacticalPhase.RespondRedeem(redeemedCard);
            Debug.Print(string.Format("Activated {0}", redeemedCard.Guid));
            Debug.Print(string.Format("AssistCandidates:{0}"
                , string.Join(",", tacticalPhase.RedeemCandidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageSelectCards(string[] parts)
        {
            var selectCards = m_remoteInteraction as Interactions.SelectCards;
            if (selectCards == null)
            {
                throw new Exception("Wrong Phase");
            }

            if (parts.Length == 2)
            {
                selectCards.Respond(Indexable.Empty<CardInstance>());
                Debug.Print("Selected nothing");
            }
            else
            {
                var selectedCards = new List<CardInstance>();
                for (int i = 2; i < parts.Length; i++)
                {
                    var selectedCard = selectCards.Candidates[Convert.ToInt32(parts[i])];
                    selectedCards.Add(selectedCard);
                    Debug.Print(string.Format("Selected {0}", selectedCard.Guid));
                }
                selectCards.Respond(selectedCards.ToIndexable());
            }

            Debug.Print(string.Format("SelectCandidates:{0}",
                string.Join(",", selectCards.Candidates.Select(candidate => candidate.Guid.ToString()))));
            m_remoteInteraction = null;
        }

        private void InterpretMessageSelectNumber(string[] parts)
        {
            var selectNumber = m_remoteInteraction as Interactions.SelectNumber;
            if (selectNumber == null)
            {
                throw new Exception("Wrong Phase");
            }

            if (parts[2] == "null")
            {
                selectNumber.Respond(null);
                Debug.Print("SelectNumber: null");
            }
            else
            {
                selectNumber.Respond(Int32.Parse(parts[2]));
                Debug.Print(string.Format("SelectNumber: {0}", parts[2]));
            }
            m_remoteInteraction = null;
        }

        #endregion
    }
}
