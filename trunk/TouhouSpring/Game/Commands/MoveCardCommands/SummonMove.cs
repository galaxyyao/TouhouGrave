using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SummonMove<TToZone> : BaseCommand,
        IMoveTo<TToZone>
        where TToZone : IZoneToken, new()
    {
        private static TToZone s_toZone = new TToZone();

        public CardInstance Subject { get; private set; }
        public int FromZone { get { return SystemZone.Library; } }
        public int ToZone { get { return s_toZone.Zone; } }

        public Player Owner
        {
            get; private set;
        }

        public ICardModel Model
        {
            get; private set;
        }

        public SummonMove(Player owner, ICardModel model)
            : this(owner, model, null)
        { }

        public SummonMove(Player owner, ICardModel model, ICause cause)
            : base(cause)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            else if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Owner = owner;
            Model = model;
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
            Subject = new CardInstance(Model, Owner);
            s_toZone.Add(Subject);
            Subject.Zone = s_toZone.Zone;
        }
    }
}
