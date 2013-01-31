using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Choice
    {
        public virtual void Make(Interactions.BaseInteraction io) { }
        public virtual string Print(Interactions.BaseInteraction io) { return ""; }
    }

    class PlayCardChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public PlayCardChoice(int cardIndex)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondPlay(tacticalPhase.PlayCardCandidates[CardIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Play: " + (io as Interactions.TacticalPhase).PlayCardCandidates[CardIndex].Model.Name;
        }
    }

    class ActivateAssistChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public ActivateAssistChoice(int cardIndex)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondActivate(tacticalPhase.ActivateAssistCandidates[CardIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Activate: " + (io as Interactions.TacticalPhase).ActivateAssistCandidates[CardIndex].Model.Name;
        }
    }

    class CastSpellChoice : Choice
    {
        public int SpellIndex
        {
            get; private set;
        }

        public CastSpellChoice(int spellIndex)
        {
            SpellIndex = spellIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondCast(tacticalPhase.CastSpellCandidates[SpellIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Cast: " + (io as Interactions.TacticalPhase).CastSpellCandidates[SpellIndex].Model.Name;
        }
    }

    class PassChoice : Choice
    {
        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondPass();
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Pass";
        }
    }

    class SelectCardChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public SelectCardChoice(int cardIndex)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var selectCards = io as Interactions.SelectCards;
            selectCards.Respond(new BaseCard[] { selectCards.SelectFromSet[CardIndex] }.ToIndexable());
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Select: " + (io as Interactions.SelectCards).SelectFromSet[CardIndex].Model.Name;
        }
    }
}
