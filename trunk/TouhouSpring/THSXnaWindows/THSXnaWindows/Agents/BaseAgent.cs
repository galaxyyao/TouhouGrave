﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class BaseAgent
    {
        public int PlayerIndex
        {
            get; private set;
        }

        public BaseAgent(int playerIndex)
        {
            PlayerIndex = playerIndex;
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
        public virtual void OnRespondBack(Interactions.BaseInteraction io, object result) { }
        public virtual void OnInitiativeCommandEnd() { }
        public virtual void OnInitiativeCommandCanceled() { }
    }
}
