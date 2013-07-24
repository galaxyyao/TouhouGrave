using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Transform : BaseCommand
    {
        public CardInstance CardToTransform
        {
            get; private set;
        }

        public ICardModel NewCardModel
        {
            get; private set;
        }

        public Transform(CardInstance cardToTransform, ICardModel newModel)
        {
            if (cardToTransform == null)
            {
                throw new ArgumentNullException("cardToTransform");
            }
            else if (newModel == null)
            {
                throw new ArgumentNullException("newModel");
            }
            else if (newModel == cardToTransform.Model)
            {
                throw new ArgumentException("Card will not be transformed.");
            }

            CardToTransform = cardToTransform;
            NewCardModel = newModel;
        }

        internal override void ValidateOnIssue()
        {
            Validate(CardToTransform);
        }

        internal override bool ValidateOnRun()
        {
            return !CardToTransform.IsDestroyed;
        }

        internal override void RunMain()
        {
            CardToTransform.Reset(NewCardModel);
        }
    }
}
