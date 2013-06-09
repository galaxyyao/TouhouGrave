using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Directly play a card onto the battlefield
    /// </summary>
    public class Summon : BaseCommand
    {
        // TODO: change to serializable behavior ID
        public ICardModel Model
        {
            get; private set;
        }

        public Player Owner
        {
            get; private set;
        }

        public CardInstance CardSummoned
        {
            get; private set;
        }

        public Summon(ICardModel model, Player owner)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            else if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            Model = model;
            Owner = owner;
        }

        internal override void ValidateOnIssue()
        {
            if (Model == null)
            {
                FailValidation("Card model can't be null.");
            }
            Validate(Owner);
        }

        internal override bool ValidateOnRun()
        {
            return true;
        }

        internal override void RunMain()
        {
            CardSummoned = new CardInstance(Model, Owner);
            Owner.m_battlefieldCards.Add(CardSummoned);
            Context.Game.SubscribeCardToCommands(CardSummoned);
        }
    }
}
