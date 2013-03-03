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
        private List<string> _messageQueue = new List<string>();



        public void Connect(string host, int port)
        {
            _client.Start();
            _client.Connect(host, port);
        }

        public void Disconnect()
        {
            _client.Disconnect("Disconnect request by user");
            _client.Shutdown("bye");
        }

        private void SendMessage(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
        }

        private void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = _client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            string text = im.ReadString();
                            //Output(text);
                            Debug.Print(text);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string text = status.ToString() + im.ReadString();
                            Debug.Print(text);

                            if (status == NetConnectionStatus.Connected)
                            {
                                //TODO: Show "Connected" information on UI
                                ;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //TODO: Show "Lost Connection" Information on UI
                                //TODO: Abort Game Thread and return to Main Menu
                                ;
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            string text = im.ReadString();
                            Debug.Print(text);
                            InterpretMessage(text);
                        }
                        break;
                    default:
                        Debug.Print("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        private void InterpretMessage(string message)
        {
            List<string> parts = message.Split(' ').ToList();
            switch (parts[1])
            {
                case "enterroom":
                    {
                        RoomId = Convert.ToInt32(parts[0]);
                        Seed = -1;
                        SendMessage(string.Format("{0} {1}", RoomId, "roomentered"));
                    }
                    break;
                case "disconnect":
                    var game = GameApp.Service<Services.GameManager>().Game;
                    break;
                case "startgame":
                    {
                        RoomStatus = RoomStatusEnum.Starting;
                        StartupIndex = Convert.ToInt32(parts[2]);
                        SendMessage(string.Format("{0} {1}", RoomId, "gamestarted"));
                    }
                    break;
                case "generateseed":
                    {
                        Seed = Convert.ToInt32(parts[2]);
                    }
                    break;
                case "switchturn":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo).RespondPass();
                        CurrentIo = null;
                    }
                    break;
                case "sacrifice":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondSacrifice(((Interactions.TacticalPhase)CurrentIo).SacrificeCandidates[Convert.ToInt32(parts[2])]);
                        CurrentIo = null;
                    }
                    break;
                case "playcard":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondPlay(((Interactions.TacticalPhase)CurrentIo).PlayCardCandidates[Convert.ToInt32(parts[2])]);
                        CurrentIo = null;
                    }
                    break;
                case "attackcard":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondAttackCard(((Interactions.TacticalPhase)CurrentIo).AttackerCandidates[Convert.ToInt32(parts[2])]
                            , ((Interactions.TacticalPhase)CurrentIo).DefenderCandidates[Convert.ToInt32(parts[3])]);
                        CurrentIo = null;
                    }
                    break;
                case "attackplayer":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondAttackPlayer(((Interactions.TacticalPhase)CurrentIo).AttackerCandidates[Convert.ToInt32(parts[2])]
                            , CurrentIo.Game.ActingPlayerEnemies.FirstOrDefault());
                        CurrentIo = null;
                    }
                    break;
                case "activateassist":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondActivate(((Interactions.TacticalPhase)CurrentIo).ActivateAssistCandidates[Convert.ToInt32(parts[2])]);
                        CurrentIo = null;
                    }
                    break;
                case "castspell":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondCast(((Interactions.TacticalPhase)CurrentIo).CastSpellCandidates[Convert.ToInt32(parts[2])]);
                        CurrentIo = null;
                    }
                    break;
                case "selectcards":
                    {
                        if (CurrentIo == null)
                        {
                            RemoteCommand command = new RemoteCommand
                            {
                                RemoteAction = RemoteCommand.RemoteActionEnum.SelectCards,
                            };
                            List<int> parameters = new List<int>();
                            for (int i = 3; i < parts.Count; i++)
                            {
                                parameters.Add(Convert.ToInt32(parts[i]));
                            }
                            command.ResultParameters = parameters.ToArray();
                            RemoteCommandEnqueue(command);
                            List<CardInstance> selectedCards = new List<CardInstance>();
                            for (int i = 3; i < parts.Count; i++)
                            {
                                selectedCards.Add(((Interactions.SelectCards)CurrentIo).Candidates[Convert.ToInt32(parts[i])]);
                            }
                        }
                        else
                        {
                            if (!(CurrentIo is Interactions.SelectCards))
                                throw new Exception("Wrong Phase");
                            List<CardInstance> selectedCards = new List<CardInstance>();
                            for (int i = 3; i < parts.Count; i++)
                            {
                                selectedCards.Add(((Interactions.SelectCards)CurrentIo).Candidates[Convert.ToInt32(parts[i])]);
                            }
                            ((Interactions.SelectCards)CurrentIo)
                                .Respond(selectedCards.ToIndexable());
                            CurrentIo = null;
                        }
                    }
                    break;
                case "redeem":
                    {
                        if (CurrentIo == null)
                            throw new Exception("current io is null");
                        if (!(CurrentIo is Interactions.TacticalPhase))
                            throw new Exception("Wrong Phase");
                        ((Interactions.TacticalPhase)CurrentIo)
                            .RespondRedeem(((Interactions.TacticalPhase)CurrentIo).RedeemCandidates[Convert.ToInt32(parts[2])]);
                        CurrentIo = null;
                    }
                    break;
                default:
                    Debug.Print("Invalid argument");
                    break;
            }
        }


        public void EnqueueMessage(string message)
        {
            _messageQueue.Add(message);
        }

        public void DequeueMessage()
        {
            foreach (string message in _messageQueue)
            {
                SendMessage(message);
            }
            _messageQueue.Clear();
        }

        public void ClearQueue()
        {
            _messageQueue.Clear();
        }
    }
}
