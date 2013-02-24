using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Network = TouhouSpring.Network;

namespace TouhouSpring.Agents
{
    class NetworkLocalPlayerAgent : BaseAgent
    {
        Network.Client _client = null;

        public NetworkLocalPlayerAgent(Network.Client client)
        {
            _client = client;
        }

        public override bool OnCardPlayCanceled(Interactions.NotifyCardEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.ModalDialog>().Show(io.Message, () =>
                {
                    io.Respond();
                    // send message of "OnCardPlayCanceled respond";
                });
                return true;
            }

            return false;
        }

        public override bool OnSpellCastCanceled(Interactions.NotifySpellEvent io)
        {
            if (!String.IsNullOrEmpty(io.Message))
            {
                GameApp.Service<Services.ModalDialog>().Show(io.Message, () => io.Respond());
                return true;
            }

            return false;
        }

        public override bool OnTurnEnded(Interactions.NotifyPlayerEvent io)
        {
            _client.SendMessage(string.Format("{0} switchturn", _client.RoomId));
            return false;
        }


        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            SendCommand(io);
            GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.TacticalPhase(), io);
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            SendCommand(io);
            GameApp.Service<Services.GameUI>().EnterState(new Services.UIStates.SelectCards(), io);
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            // translate from Interactions.MessageBox.Button to Services.ModalDialog.Button
            Services.ModalDialog.Button buttons = 0;
            if ((io.Buttons & Interactions.MessageBoxButtons.OK) != 0)
            {
                buttons |= Services.ModalDialog.Button.OK;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Cancel) != 0)
            {
                buttons |= Services.ModalDialog.Button.Cancel;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.Yes) != 0)
            {
                buttons |= Services.ModalDialog.Button.Yes;
            }
            if ((io.Buttons & Interactions.MessageBoxButtons.No) != 0)
            {
                buttons |= Services.ModalDialog.Button.No;
            }
            GameApp.Service<Services.ModalDialog>().Show(io.Text, buttons, btn =>
            {
                // translate back...
                Interactions.MessageBoxButtons ibtn;
                switch (btn)
                {
                    case Services.ModalDialog.Button.OK:
                        ibtn = Interactions.MessageBoxButtons.OK;
                        break;
                    case Services.ModalDialog.Button.Cancel:
                        ibtn = Interactions.MessageBoxButtons.Cancel;
                        break;
                    case Services.ModalDialog.Button.Yes:
                        ibtn = Interactions.MessageBoxButtons.Yes;
                        break;
                    case Services.ModalDialog.Button.No:
                        ibtn = Interactions.MessageBoxButtons.No;
                        break;
                    default:
                        throw new ArgumentException("btn");
                }
                io.Respond(ibtn);
            });
        }

        private bool SendCommand(Interactions.BaseInteraction io)
        {
            var n = io.GetType().Name;
            switch (io.GetType().Name)
            {
                case "TacticalPhase":
                    {
                        switch (Game.CurrentCommand.Type)
                        {
                            case Game.CurrentCommand.InteractionType.TacticalPhase:
                                {
                                    Interactions.TacticalPhase.Result currentCommand = new Interactions.TacticalPhase.Result();
                                    if (Game.CurrentCommand.Result == null)
                                        return true;
                                    currentCommand = (Interactions.TacticalPhase.Result)Game.CurrentCommand.Result;
                                    //if (currentCommand.Data != null)
                                    //{
                                        switch (currentCommand.ActionType)
                                        {
                                            case Interactions.TacticalPhase.Action.Sacrifice:
                                                {
                                                    _client.SendMessage(string.Format("{0} sacrifice {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.PlayCard:
                                                {
                                                    _client.SendMessage(string.Format("{0} playcard {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.AttackCard:
                                                {
                                                    _client.SendMessage(string.Format("{0} attackcard {1} {2}"
                                                        , _client.RoomId, Game.CurrentCommand.ResultSubjectIndex
                                                        , Game.CurrentCommand.ResultParameters[0]));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.AttackPlayer:
                                                {
                                                    _client.SendMessage(string.Format("{0} attackplayer {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.ActivateAssist:
                                                {
                                                    _client.SendMessage(string.Format("{0} activateassist {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.CastSpell:
                                                {
                                                    _client.SendMessage(string.Format("{0} castspell {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            case Interactions.TacticalPhase.Action.Redeem:
                                                {
                                                    _client.SendMessage(string.Format("{0} redeem {1}", _client.RoomId, Game.CurrentCommand.ResultSubjectIndex));
                                                }
                                                break;
                                            default:
                                                break;
                                        //}
                                    }
                                }
                                break;
                            case Game.CurrentCommand.InteractionType.SelectCards:
                                {
                                    int[] selectedCardsIndexs = Game.CurrentCommand.ResultParameters;
                                    StringBuilder indexes = new StringBuilder();
                                    for (int i = 0; i < selectedCardsIndexs.Count(); i++)
                                    {
                                        indexes.Append(selectedCardsIndexs[i]);
                                        indexes.Append(" ");
                                    }
                                    indexes.Remove(indexes.Length - 1, 1);
                                    _client.SendMessage(string.Format("{0} selectcards -1 {1}", _client.RoomId, indexes.ToString()));
                                }
                                break;
                            case Game.CurrentCommand.InteractionType.Others:
                                return true;
                        }
                        
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
