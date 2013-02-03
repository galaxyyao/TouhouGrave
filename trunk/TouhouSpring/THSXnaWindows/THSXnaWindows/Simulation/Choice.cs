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

    class SacrificeChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public SacrificeChoice(int cardIndex)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondSacrifice(tacticalPhase.SacrificeCandidates[CardIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Sacrifice: " + (io as Interactions.TacticalPhase).SacrificeCandidates[CardIndex].Model.Name;
        }
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

    class RedeemChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public RedeemChoice(int cardIndex)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondRedeem(tacticalPhase.RedeemCandidates[CardIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Redeem: " + (io as Interactions.TacticalPhase).RedeemCandidates[CardIndex].Model.Name;
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

    class AttackCardChoice : Choice
    {
        public int AttackerIndex
        {
            get; private set;
        }

        public int DefenderIndex
        {
            get; private set;
        }

        public AttackCardChoice(int attackerIndex, int defenderIndex)
        {
            AttackerIndex = attackerIndex;
            DefenderIndex = defenderIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondAttackCard(tacticalPhase.AttackerCandidates[AttackerIndex], tacticalPhase.DefenderCandidates[DefenderIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            return "Attack: " + tacticalPhase.AttackerCandidates[AttackerIndex].Model.Name
                    + "->" + tacticalPhase.DefenderCandidates[DefenderIndex].Model.Name;
        }
    }

    class AttackPlayerChoice : Choice
    {
        public int AttackerIndex
        {
            get; private set;
        }

        public int PlayerIndex
        {
            get; private set;
        }

        public AttackPlayerChoice(int attackerIndex, int playerIndex)
        {
            AttackerIndex = attackerIndex;
            PlayerIndex = playerIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondAttackPlayer(tacticalPhase.AttackerCandidates[AttackerIndex], tacticalPhase.Game.Players[PlayerIndex]);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            return "AttackPlayer: " + tacticalPhase.AttackerCandidates[AttackerIndex].Model.Name
                    + "->" + tacticalPhase.Game.Players[PlayerIndex].Name;
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
            selectCards.Respond(new BaseCard[] { selectCards.Candidates[CardIndex] }.ToIndexable());
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Select: " + (io as Interactions.SelectCards).Candidates[CardIndex].Model.Name;
        }
    }
}
