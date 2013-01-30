using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Choice
    {
        public virtual void Make(Interactions.BaseInteraction io) { }
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
    }

    class PassChoice : Choice
    {
        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondPass();
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
    }
}
