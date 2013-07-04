using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class BaseAgent
    {
        private StreamWriter m_recording;

        public int PlayerIndex
        {
            get; private set;
        }

        protected BaseAgent(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        protected BaseAgent(int playerIndex, int initialSeedValue)
            : this(playerIndex)
        {
            m_recording = new StreamWriter(new FileStream("record.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8);
            m_recording.AutoFlush = true;
            m_recording.WriteLine(initialSeedValue.ToString());
        }

        // return true to indicate the agent has taken over the interaction
        // false by default.
        public virtual bool OnCardPlayCanceled(Interactions.NotifyCardEvent io) { return false; }
        public virtual bool OnSpellCastCanceled(Interactions.NotifySpellEvent io) { return false; }
        public virtual bool OnTurnStarted(Interactions.NotifyPlayerEvent io) { return false; }
        public virtual bool OnTurnEnded(Interactions.NotifyPlayerEvent io) { return false; }
        public virtual void OnTacticalPhase(Interactions.TacticalPhase io) { }
        public virtual void OnSelectCards(Interactions.SelectCards io) { }
        public virtual void OnMessageBox(Interactions.MessageBox io) { }
        public virtual void OnSelectNumber(Interactions.SelectNumber io) { }
        public virtual void OnRespondBack(Interactions.BaseInteraction io, object result)
        {
            if (m_recording == null)
            {
                return;
            }

            if (io is Interactions.TacticalPhase)
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
                        m_recording.WriteLine("atp:" + ((r.Data as object[])[0] as CardInstance).Guid.ToString() + ":" + tp.Game.Players.IndexOf((r.Data as object[])[1] as Player).ToString());
                        break;
                }
            }
            else if (io is Interactions.SelectCards)
            {
                m_recording.Write("se");
                foreach (var card in result as IIndexable<CardInstance>)
                {
                    m_recording.Write(":" + card.Guid.ToString());
                }
                m_recording.WriteLine();
            }
            else if (io is Interactions.SelectNumber)
            {
                var si = result as int?;
                m_recording.WriteLine("sn:" + (si == null ? "null" : si.ToString()));
            }
        }
        public virtual void OnInitiativeCommandEnd() { }
        public virtual void OnInitiativeCommandCanceled() { }
    }
}
