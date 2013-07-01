using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TouhouSpring.Services;
using TouhouSpring.UI;
using THSNetwork = TouhouSpring.Network;

namespace TouhouSpring
{
    class XnaUIController : BaseController
    {
        private Agents.BaseAgent[] m_agents;
        private List<int> m_destroyedCards = new List<int>();

        private StreamWriter m_recording;
        private StreamReader m_playingBack;

        public int RandomSeed
        {
            get; private set;
        }

        public IIndexable<Agents.BaseAgent> Agents
        {
            get; private set;
        }

        public IEnumerable<int> DestroyedCards
        {
            get { return m_destroyedCards; }
        }

        public XnaUIController(Agents.BaseAgent[] agents, int seed)
        {
            m_agents = agents;
            Agents = m_agents.ToIndexable();
            RandomSeed = seed;

            if (GameApp.Instance.GetCommandLineArgValue("record") != null)
            {
                m_recording = new StreamWriter(new FileStream("record.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8);
                m_recording.AutoFlush = true;
                m_recording.WriteLine(RandomSeed.ToString());
            }
            else if (GameApp.Instance.GetCommandLineArgValue("playback") != null)
            {
                m_playingBack = new StreamReader(new FileStream("record.txt", FileMode.Open, FileAccess.Read), Encoding.UTF8);
                RandomSeed = Int32.Parse(m_playingBack.ReadLine());
            }
        }

        public override int GetRandomSeed()
        {
            return RandomSeed;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyOnly))]
        private bool OnNotified(Interactions.NotifyOnly interactionObj)
        {
            throw new InvalidOperationException("NotifyOnly shall not occur.");
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyCardEvent))]
        private bool OnNotified(Interactions.NotifyCardEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnCardDestroyed":
                    m_destroyedCards.Add(interactionObj.Card.Guid);
                    break;
                case "OnCardPlayCanceled":
                    if (m_agents[Game.ActingPlayer.Index].OnCardPlayCanceled(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyGameEvent))]
        private bool OnNotified(Interactions.NotifyGameEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnInitiativeCommandEnd":
                    m_agents[Game.ActingPlayer.Index].OnInitiativeCommandEnd();
                    break;
                case "OnInitiativeCommandCanceled":
                    m_agents[Game.ActingPlayer.Index].OnInitiativeCommandCanceled();
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyPlayerEvent))]
        private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnTurnEnded":
                    if (m_agents[Game.ActingPlayer.Index].OnTurnEnded(interactionObj))
                    {
                        return true;
                    }
                    break;
                case "OnTurnStarted":
                    if (m_agents[Game.ActingPlayer.Index].OnTurnStarted(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifySpellEvent))]
        private bool OnNotified(Interactions.NotifySpellEvent interactionObj)
        {
            switch (interactionObj.Notification)
            {
                case "OnSpellCasted":
                    break;
                case "OnSpellCastCanceled":
                    if (m_agents[Game.ActingPlayer.Index].OnSpellCastCanceled(interactionObj))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.TacticalPhase))]
        private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            if (m_playingBack != null && !(m_agents[interactionObj.Player.Index] is Agents.AIAgent))
            {
                var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                switch (respond[0])
                {
                    case "pa":
                        interactionObj.RespondPass();
                        return false;
                    case "pl":
                        interactionObj.RespondPlay(interactionObj.PlayCardCandidates.First(c => c.Guid == Int32.Parse(respond[1])));
                        return false;
                    case "ac":
                        interactionObj.RespondActivate(interactionObj.ActivateAssistCandidates.First(c => c.Guid == Int32.Parse(respond[1])));
                        return false;
                    case "sa":
                        interactionObj.RespondSacrifice(interactionObj.SacrificeCandidates.First(c => c.Guid == Int32.Parse(respond[1])));
                        return false;
                    case "re":
                        interactionObj.RespondRedeem(interactionObj.RedeemCandidates.First(c => c.Guid == Int32.Parse(respond[1])));
                        return false;
                    case "ca":
                        interactionObj.RespondCast(interactionObj.CastSpellCandidates.First(c => c.Host.Guid == Int32.Parse(respond[1]) && c.Host.Behaviors[Int32.Parse(respond[2])] == c));
                        return false;
                    case "atc":
                        interactionObj.RespondAttackCard(interactionObj.AttackerCandidates.First(c => c.Guid == Int32.Parse(respond[1])), interactionObj.DefenderCandidates.First(c => c.Guid == Int32.Parse(respond[2])));
                        return false;
                    case "atp":
                        interactionObj.RespondAttackPlayer(interactionObj.AttackerCandidates.First(c => c.Guid == Int32.Parse(respond[1])), interactionObj.Game.Players[Int32.Parse(respond[2])]);
                        return false;
                }
            }

            m_agents[interactionObj.Player.Index].OnTacticalPhase(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            if (m_playingBack != null && !(m_agents[interactionObj.Player.Index] is Agents.AIAgent))
            {
                var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (respond[0] == "se")
                {
                    CardInstance[] cards = new CardInstance[respond.Length - 1];
                    for (int i = 1; i < respond.Length; ++i)
                    {
                        cards[i - 1] = interactionObj.Candidates.First(c => c.Guid == Int32.Parse(respond[i]));
                    }
                    interactionObj.Respond(cards.ToIndexable());
                    return false;
                }
            }

            m_agents[interactionObj.Player.Index].OnSelectCards(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            m_agents[interactionObj.Player.Index].OnMessageBox(interactionObj);
            return true;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectNumber))]
        private bool OnSelectNumber(Interactions.SelectNumber interactionObj)
        {
            if (m_playingBack != null && !(m_agents[interactionObj.Player.Index] is Agents.AIAgent))
            {
                var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (respond[0] == "sn")
                {
                    interactionObj.Respond(respond[1] == "null" ? (int?)null : Int32.Parse(respond[1]));
                    return false;
                }
            }

            m_agents[interactionObj.Player.Index].OnSelectNumber(interactionObj);
            return true;
        }

        public override void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            if (io is Interactions.TacticalPhase)
            {
                if (m_recording != null && !(m_agents[(io as Interactions.TacticalPhase).Player.Index] is Agents.AIAgent))
                {
                    var tp = io as Interactions.TacticalPhase;
                    var r = (Interactions.TacticalPhase.Result)result;
                    switch (r.ActionType)
                    {
                        case Interactions.BaseInteraction.PlayerAction.Pass:
                            m_recording.WriteLine("pa");
                            break;
                        case Interactions.BaseInteraction.PlayerAction.PlayCard:
                            m_recording.WriteLine("pl:" + (r.Data as CardInstance).Guid.ToString());
                            break;
                        case Interactions.BaseInteraction.PlayerAction.ActivateAssist:
                            m_recording.WriteLine("ac:" + (r.Data as CardInstance).Guid.ToString());
                            break;
                        case Interactions.BaseInteraction.PlayerAction.CastSpell:
                            m_recording.WriteLine("ca:" + (r.Data as Behaviors.ICastableSpell).Host.Guid.ToString() + ":" + (r.Data as Behaviors.ICastableSpell).Host.Behaviors.IndexOf(r.Data as Behaviors.ICastableSpell));
                            break;
                        case Interactions.BaseInteraction.PlayerAction.Sacrifice:
                            m_recording.WriteLine("sa:" + (r.Data as CardInstance).Guid.ToString());
                            break;
                        case Interactions.BaseInteraction.PlayerAction.Redeem:
                            m_recording.WriteLine("re:" + (r.Data as CardInstance).Guid.ToString());
                            break;
                        case Interactions.BaseInteraction.PlayerAction.AttackCard:
                            m_recording.WriteLine("atc:" + (r.Data as CardInstance[])[0].Guid.ToString() + ":" + (r.Data as CardInstance[])[1].Guid.ToString());
                            break;
                        case Interactions.BaseInteraction.PlayerAction.AttackPlayer:
                            m_recording.WriteLine("atp:" + ((r.Data as object[])[0] as CardInstance).Guid.ToString() + ":" + Game.Players.IndexOf((r.Data as object[])[1] as Player).ToString());
                            break;
                    }
                }

                m_agents[(io as Interactions.TacticalPhase).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.SelectCards)
            {
                if (m_recording != null && !(m_agents[(io as Interactions.SelectCards).Player.Index] is Agents.AIAgent))
                {
                    m_recording.Write("se");
                    foreach (var card in result as IIndexable<CardInstance>)
                    {
                        m_recording.Write(":" + card.Guid.ToString());
                    }
                    m_recording.WriteLine();
                }

                m_agents[(io as Interactions.SelectCards).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.SelectNumber)
            {
                if (m_recording != null && !(m_agents[(io as Interactions.SelectNumber).Player.Index] is Agents.AIAgent))
                {
                    var si = result as int?;
                    m_recording.WriteLine("sn:" + si == null ? "null" : si.ToString());
                }

                m_agents[(io as Interactions.SelectNumber).Player.Index].OnRespondBack(io, result);
            }
            else if (io is Interactions.NotifyOnly
                || io is Interactions.NotifyCardEvent
                || io is Interactions.NotifyPlayerEvent
                || io is Interactions.NotifyGameEvent
                || io is Interactions.NotifySpellEvent
                )
            {
                //let notifyonly be
            }
            else
            {
                throw new Exception("Unhandled Response back Interation type.");
            }
        }

        public override void OnCommandFlushed()
        {
            GameApp.Service<GameManager>().RefreshGameEvaluators();
        }
    }
}
