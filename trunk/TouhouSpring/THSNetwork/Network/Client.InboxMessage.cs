using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Xml.Linq;


namespace TouhouSpring.Network
{
    public partial class Client
    {
        private Dictionary<string, Action<XDocument>> m_interactionActions = new Dictionary<string, Action<XDocument>>();

        private Interactions.BaseInteraction m_remoteInteraction;
        private Queue<string> m_interactionMessageQueue = new Queue<string>();

        public void RemoteEnterInteraction(Interactions.BaseInteraction io)
        {
            System.Diagnostics.Debug.Assert(m_remoteInteraction == null);
            m_remoteInteraction = io;
            if (m_interactionMessageQueue.Count != 0)
            {
                var msg = m_interactionMessageQueue.Dequeue();
                ProcessInteractionMessage(msg);
            }
        }

        private void InitializeInteractionActionsTable()
        {
            //Type
            m_interactionActions.Add("ServerInfo", InterpretMessageAction);
            m_interactionActions.Add("Game", InterpretMessageAction);

            //ServerInfo
            m_interactionActions.Add("ServerShutDown", InterpretMessageServerShutDown);
            m_interactionActions.Add("EnterRoom", InterpretMessageEnterRoom);
            m_interactionActions.Add("WaitingEnemy", InterpretMessageWaitingEnemy);
            m_interactionActions.Add("EnemyDisconnect", InterpretMessageEnemyDisconnect);
            m_interactionActions.Add("StartGame", InterpretMessageStartGame);
            m_interactionActions.Add("GenerateSeed", InterpretMessageEnemyGenerateSeed);

            //Game
            m_interactionActions.Add("SwitchTurn", InterpretMessageSwitchTurn);
            m_interactionActions.Add("Sacrifice", InterpretMessageSacrifice);
            m_interactionActions.Add("PlayCard", InterpretMessagePlayCard);
            m_interactionActions.Add("AttackCard", InterpretMessageAttackCard);
            m_interactionActions.Add("AttackPlayer", InterpretMessageAttackPlayer);
            m_interactionActions.Add("ActivateAssist", InterpretMessageActivateAssist);
            m_interactionActions.Add("Castspell", InterpretMessageCastSpell);
            m_interactionActions.Add("Redeem", InterpretMessageRedeem);
            m_interactionActions.Add("SelectCards", InterpretMessageSelectCards);
            m_interactionActions.Add("SelectNumber", InterpretMessageSelectNumber);
        }

        private void OnInteractionMessageArrived(string message)
        {
            if (m_remoteInteraction == null && RoomStatus == RoomStatusEnum.Started)
            {
                m_interactionMessageQueue.Enqueue(message);
            }
            else
            {
                ProcessInteractionMessage(message);
            }
        }

        private void ProcessInteractionMessage(string message)
        {
            Action<XDocument> action;
            XDocument xmlMessage = XDocument.Parse(message);
            if (m_interactionActions.TryGetValue(xmlMessage.Root.Elements("Type").FirstOrDefault().Value, out action))
            {
                action(xmlMessage);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region Interaction message actions

        #region Type
        private void InterpretMessageAction(XDocument xmlMessage)
        {
            Action<XDocument> action;
            if (m_interactionActions.TryGetValue(xmlMessage.Root.Descendants("Action").FirstOrDefault().Value, out action))
            {
                action(xmlMessage);
            }
            else
            {
                throw new NotImplementedException("The method for {0} has not been implemented.");
            }
        }
        #endregion


        #region ServerInfo

        private void InterpretMessageServerShutDown(XDocument xmlMessage)
        {
            //TODO: End game if game is running
            //TODO: Logout chat
            //TODO: Pop warning message
        }

        private void InterpretMessageEnterRoom(XDocument xmlMessage)
        {
            RoomId = XML.GetFirstDescendantsValue<Int32>(xmlMessage, "RoomId");
            Seed = -1;
        }

        private void InterpretMessageWaitingEnemy(XDocument xmlMessage)
        {
            //TODO: Show waiting information
        }

        private void InterpretMessageEnemyDisconnect(XDocument xmlMessage)
        {
            //TODO: show enemy disconnect information
            //TODO: abort game
        }

        private void InterpretMessageStartGame(XDocument xmlMessage)
        {
            RoomStatus = RoomStatusEnum.Starting;
            StartupIndex = XML.GetFirstDescendantsValue<Int32>(xmlMessage, "StartGameIndex");
        }

        private void InterpretMessageEnemyGenerateSeed(XDocument xmlMessage)
        {
            Seed = XML.GetFirstDescendantsValue<Int32>(xmlMessage, "Seed");
        }
        #endregion

        #region Game
        private void InterpretMessageSwitchTurn(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            tacticalPhase.RespondPass();
            m_remoteInteraction = null;
        }

        private void InterpretMessageSacrifice(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var sacrificedCard = tacticalPhase.SacrificeCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "SacrificeIndex")];
            tacticalPhase.RespondSacrifice(sacrificedCard);
            m_remoteInteraction = null;
        }

        private void InterpretMessagePlayCard(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var playedCard = tacticalPhase.PlayCardCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "PlayCardIndex")];
            tacticalPhase.RespondPlay(playedCard);
            m_remoteInteraction = null;
        }

        private void InterpretMessageAttackCard(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var attackerCard = tacticalPhase.AttackerCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "AttackerIndex")];
            var defenderCard = tacticalPhase.DefenderCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "DefenderIndex")];
            tacticalPhase.RespondAttackCard(attackerCard, defenderCard);
            m_remoteInteraction = null;
        }

        private void InterpretMessageAttackPlayer(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var attackerCard = tacticalPhase.AttackerCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "AttackerIndex")];
            var playerBeingAttacked = tacticalPhase.Game.Players[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "PlayerIndex")];
            tacticalPhase.RespondAttackPlayer(attackerCard, playerBeingAttacked);
            m_remoteInteraction = null;
        }

        private void InterpretMessageActivateAssist(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var assistCard = tacticalPhase.ActivateAssistCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "AssistIndex")];
            tacticalPhase.RespondActivate(assistCard);
            m_remoteInteraction = null;
        }

        private void InterpretMessageCastSpell(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var spell = tacticalPhase.CastSpellCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "CastSpellIndex")];
            tacticalPhase.RespondCast(spell);
            m_remoteInteraction = null;
        }

        private void InterpretMessageRedeem(XDocument xmlMessage)
        {
            var tacticalPhase = m_remoteInteraction as Interactions.TacticalPhase;
            if (tacticalPhase == null)
            {
                throw new Exception("Wrong Phase");
            }

            var redeemedCard = tacticalPhase.RedeemCandidates[XML.GetFirstDescendantsValue<Int32>(xmlMessage, "RedeemIndex")];
            tacticalPhase.RespondRedeem(redeemedCard);
            m_remoteInteraction = null;
        }

        private void InterpretMessageSelectCards(XDocument xmlMessage)
        {
            var selectCards = m_remoteInteraction as Interactions.SelectCards;
            if (selectCards == null)
            {
                throw new Exception("Wrong Phase");
            }

            List<int> indexes = XML.GetDescendantsValues<Int32>(xmlMessage, "Index");
            if (indexes.Count() == 0)
            {
                selectCards.Respond(Indexable.Empty<CardInstance>());
            }
            else
            {
                var selectedCards = new List<CardInstance>();
                for (int i = 0; i < indexes.Count; i++)
                {
                    var selectedCard = selectCards.Candidates[indexes[i]];
                    selectedCards.Add(selectedCard);
                }
                selectCards.Respond(selectedCards.ToIndexable());
            }
            m_remoteInteraction = null;
        }

        private void InterpretMessageSelectNumber(XDocument xmlMessage)
        {
            var selectNumber = m_remoteInteraction as Interactions.SelectNumber;
            if (selectNumber == null)
            {
                throw new Exception("Wrong Phase");
            }

            string num = XML.GetFirstDescendantsValue<string>(xmlMessage, "Number");
            if (string.IsNullOrEmpty(num))
            {
                selectNumber.Respond(null);
            }
            else
            {
                int number;
                if (!Int32.TryParse(num, out number))
                {
                    throw new InvalidCastException("Invalid Number");
                }
                selectNumber.Respond(number);
            }
            m_remoteInteraction = null;
        }
        #endregion
        #endregion
    }
}
