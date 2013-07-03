using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Choice
    {
        public int Order
        {
            get; private set;
        }

#if DEBUG
        public string DebugName;
#endif

        public virtual void Make(Interactions.BaseInteraction io) { }
        public virtual string Print(Interactions.BaseInteraction io) { return ""; }

        protected Choice(int order)
        {
            Order = order;
        }
    }

    class KillBranchChoice : Choice
    {
        public KillBranchChoice() : base(10000)
        { }

        public override void Make(Interactions.BaseInteraction io)
        {
            (io as Interactions.TacticalPhase).RespondPass();
        }

        public override int GetHashCode()
        {
            return "Kill".GetHashCode();
        }
    }

    class SacrificeChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public ICardModel CardModel
        {
            get; private set;
        }

        public SacrificeChoice(int cardIndex, ICardModel cardModel) : base(1)
        {
            CardIndex = cardIndex;
            CardModel = cardModel;
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

        public override int GetHashCode()
        {
            return CardIndex.GetHashCode() ^ CardModel.Id.GetHashCode();
        }
    }

    class PlayCardChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public int CardGuid
        {
            get; private set;
        }

        public bool IsWarrior
        {
            get; private set;
        }

        public PlayCardChoice(int cardIndex, int cardGuid, bool isWarrior) : base(4)
        {
            CardIndex = cardIndex;
            CardGuid = cardGuid;
            IsWarrior = isWarrior;
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

        public override int GetHashCode()
        {
            return "Play".GetHashCode() ^ CardGuid.GetHashCode();
        }
    }

    class RedeemChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public int CardGuid
        {
            get; private set;
        }

        public RedeemChoice(int cardIndex, int cardGuid) : base(2)
        {
            CardIndex = cardIndex;
            CardGuid = cardGuid;
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

        public override int GetHashCode()
        {
            return "Redeem".GetHashCode() ^ CardGuid.GetHashCode();
        }
    }

    class ActivateAssistChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public int CardGuid
        {
            get; private set;
        }

        public ActivateAssistChoice(int cardIndex, int cardGuid) : base(4)
        {
            CardIndex = cardIndex;
            CardGuid = cardGuid;
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

        public override int GetHashCode()
        {
            return "Activate".GetHashCode() ^ CardGuid.GetHashCode();
        }
    }

    class CastSpellChoice : Choice
    {
        public int SpellIndex
        {
            get; private set;
        }

        public CastSpellChoice(int spellIndex) : base(4)
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

        public override int GetHashCode()
        {
            return "Cast".GetHashCode() ^ SpellIndex.GetHashCode();
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

        public int DefenderGuid
        {
            get; private set;
        }

        public AttackCardChoice(int attackerIndex, int defenderIndex, int defenderGuid) : base(5)
        {
            AttackerIndex = attackerIndex;
            DefenderIndex = defenderIndex;
            DefenderGuid = defenderGuid;
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

        public override int GetHashCode()
        {
            return "Attack".GetHashCode() ^ AttackerIndex.GetHashCode() ^ DefenderGuid.GetHashCode();
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

        public AttackPlayerChoice(int attackerIndex, int playerIndex) : base(5)
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

        public override int GetHashCode()
        {
            return "AttackPlayer".GetHashCode() ^ AttackerIndex.GetHashCode() ^ PlayerIndex.GetHashCode();
        }
    }

    class PassChoice : Choice
    {
        public PassChoice() : base(6) { }

        public override void Make(Interactions.BaseInteraction io)
        {
            var tacticalPhase = io as Interactions.TacticalPhase;
            tacticalPhase.RespondPass();
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "Pass";
        }

        public override int GetHashCode()
        {
            return "Pass".GetHashCode();
        }
    }

    class SelectCardChoice : Choice
    {
        public int CardIndex
        {
            get; private set;
        }

        public SelectCardChoice(int cardIndex) : base(-1)
        {
            CardIndex = cardIndex;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var selectCards = io as Interactions.SelectCards;
            selectCards.Respond(new CardInstance[] { selectCards.Candidates[CardIndex] }.ToIndexable());
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "SelectCard: " + (io as Interactions.SelectCards).Candidates[CardIndex].Model.Name;
        }

        public override int GetHashCode()
        {
            return "SelectCard".GetHashCode() ^ CardIndex.GetHashCode();
        }
    }

    class SelectNumberChoice : Choice
    {
        public int Selection
        {
            get; private set;
        }

        public SelectNumberChoice(int selection) : base(-1)
        {
            Selection = selection;
        }

        public override void Make(Interactions.BaseInteraction io)
        {
            var selectNumber = io as Interactions.SelectNumber;
            selectNumber.Respond(Selection);
        }

        public override string Print(Interactions.BaseInteraction io)
        {
            return "SelectNumber: " + Selection.ToString();
        }

        public override int GetHashCode()
        {
            return "SelectNumber".GetHashCode() ^ Selection.GetHashCode();
        }
    }
}
