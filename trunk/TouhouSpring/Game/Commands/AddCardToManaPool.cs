using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Add a instance of cardmodel to manapool
    /// </summary>
    public class AddCardToManaPool : BaseCommand
    {
        // TODO: change to serializable behavior ID
        public ICardModel Model
        {
            get;
            private set;
        }

        public Player Owner
        {
            get;
            private set;
        }

        public BaseCard CardToAdd
        {
            get;
            private set;
        }

        public AddCardToManaPool(ICardModel model, Player owner)
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

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            CardToAdd = new BaseCard(Model, Owner);
            Owner.AddToSacrificeSorted(CardToAdd);
        }
    }
}
