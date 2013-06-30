using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class CardInstance
    {
        public ICardModel Model
        {
            get; private set;
        }

        public Player Owner
        {
            get; private set;
        }

        public Int32 Guid
        {
            get; private set;
        }

        public int Zone
        {
            get; internal set;
        }

        public bool IsOnBattlefield
        {
            get { return Zone == SystemZone.Battlefield; }
        }

        public bool IsActivatedAssist
        {
            get { return Owner.ActivatedAssits.Contains(this); }
        }

        public bool IsDestroyed
        {
            get { return Zone == SystemZone.Graveyard; }
        }

        internal CardInstance(ICardModel model, Player owner)
            : this(model, owner, true)
        { }

        private CardInstance()
        {
            Behaviors = new Behaviors.BehaviorList(this);
        }

        private CardInstance(ICardModel model, Player owner, bool hasGuid) : this()
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
            Guid = hasGuid ? owner.Game.GenerateNextCardGuid() : 0;
            Model.Behaviors.ForEach(bhv => Behaviors.Add((bhv as Behaviors.IInternalBehaviorModel).CreateInitializedPersistent()));
        }
    }
}
