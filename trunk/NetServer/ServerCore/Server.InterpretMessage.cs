using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Xml.Linq;
using Common;

namespace TouhouSpring.ServerCore
{
    public partial class Server
    {
        private Dictionary<string, Action<NetConnection, XDocument>> m_messageActions = new Dictionary<string, Action<NetConnection, XDocument>>();

        private void InitializeMessageActionsTable()
        {
            //Type
            m_messageActions.Add("ServerInfo", InterpretMessageType);
            m_messageActions.Add("Game", InterpretMessageType);

            //ServerInfo
            m_messageActions.Add("Login", InterpretMessageLogin);

            //Game
            m_messageActions.Add("StartRandomGame", InterpretMessageStartRandomGame);
            m_messageActions.Add("SwitchTurn", InterpretMessagePassToEnemy);
            m_messageActions.Add("Sacrifice", InterpretMessagePassToEnemy);
            m_messageActions.Add("PlayCard", InterpretMessagePassToEnemy);
            m_messageActions.Add("AttackCard", InterpretMessagePassToEnemy);
            m_messageActions.Add("AttackPlayer", InterpretMessagePassToEnemy);
            m_messageActions.Add("ActivateAssist", InterpretMessagePassToEnemy);
            m_messageActions.Add("Redeem", InterpretMessagePassToEnemy);
            m_messageActions.Add("SelectCards", InterpretMessagePassToEnemy);
            m_messageActions.Add("SelectNumber", InterpretMessagePassToEnemy);
        }

        #region Type
        private void InterpretMessageType(NetConnection senderConn, XDocument xmlMessage)
        {
            Action<NetConnection, XDocument> action;
            if (m_messageActions.TryGetValue(xmlMessage.Root.Descendants("Action").FirstOrDefault().Value, out action))
            {
                action(senderConn, xmlMessage);
            }
            else
            {
                Log4NetHelper.Instance.WriteErrorLog(
                    string.Format("The method for {0} has not been implemented."
                    , xmlMessage.Root.Elements("Type").FirstOrDefault().Value));
            }
        }
        #endregion

        #region ServerInfo
        private void InterpretMessageLogin(NetConnection senderConn, XDocument xmlMessage)
        {
            //TODO: Respond to user login
        }
        #endregion

        #region Game
        private void InterpretMessageStartRandomGame(NetConnection senderConn, XDocument xmlMessage)
        {
            int enteredRoomId = UserEnter(senderConn);
            SendMessage(senderConn, string.Format(
                    "<Message><Type>Game</Type><Time>{1}</Time><Info><Action>EnterRoom</Action><RoomId>{0}</RoomId></Info></Message>"
                    , enteredRoomId
                    , DateTime.Now));
            if (_roomList[enteredRoomId].Status == Room.RoomStatus.Idle)
            {
                SendMessage(senderConn, string.Format(
                    "<Message><Type>Game</Type><Time>{0}</Time><Info><Action>WaitingEnemy</Action></Info></Message>"
                    , DateTime.Now));
            }
            else
            {
                int seed = gameStartSeed.Next();
                SendMessage(enteredRoomId, string.Format(
                    "<Message><Type>Game</Type><Time>{1}</Time><Info><Action>GenerateSeed</Action><Seed>{0}</Seed></Info></Message>"
                    , seed
                    , DateTime.Now));
                foreach (var playerConn in _roomList[enteredRoomId].PlayerConns)
                {
                    //Random who start the 1st turn
                    SendMessage(playerConn
                        , string.Format("<Message><Type>Game</Type><Time>{1}</Time><Info><Action>StartGame</Action><StartGameIndex>{0}</StartGameIndex></Info></Message>"
                        , Math.Abs(_roomList[enteredRoomId].GetPlayerIndex(playerConn.RemoteUniqueIdentifier) - seed % 2)
                        , DateTime.Now));
                    //SendMessage(playerConn
                    //    , string.Format("startgame {0} {1}", enteredRoomId
                    //    , _roomList[enteredRoomId].GetPlayerIndex(playerConn.RemoteUniqueIdentifier)));//Temporarily let playerindex be the same as enter room order

                }
            }
        }

        private void InterpretMessagePassToEnemy(NetConnection senderConn, XDocument xmlMessage)
        {
            int roomId = GetRoomIdByUid(senderConn.RemoteUniqueIdentifier);
            SendMessage(_roomList[roomId].GetOpponentPlayerConnection(senderConn.RemoteUniqueIdentifier), xmlMessage.ToString());
        }
        #endregion

        private void InterpretMessage(NetConnection senderConn, string message)
        {
            Action<NetConnection, XDocument> action;
            XDocument xmlMessage = null ;
            try
            {
                xmlMessage = XDocument.Parse(message);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Instance.WriteErrorLog(ex.Message);
                return;
            }
            if (m_messageActions.TryGetValue(xmlMessage.Root.Elements("Type").FirstOrDefault().Value, out action))
            {
                action(senderConn, xmlMessage);
            }
            else
            {
                Log4NetHelper.Instance.WriteErrorLog(
                    string.Format("The method for {0} has not been implemented."
                    , xmlMessage.Root.Elements("Type").FirstOrDefault().Value));
            }
        }
    }
}
