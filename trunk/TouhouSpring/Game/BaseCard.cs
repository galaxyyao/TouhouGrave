using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// Base class for all cards with basic properties like name and type.
    /// </summary>
    public partial class BaseCard
    {
        public ICardModel Model
        {
            get; private set;
        }

        public Player Owner
        {
            get; private set;
        }

        public BaseCard(ICardModel model, Player owner)
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
            Behaviors = new Behaviors.BehaviorList(this);
            Model.Behaviors.ForEach(bhv => Behaviors.Add(bhv.InstantiatePersistent()));
        }

        public bool IsOnHand
        {
            get { return Owner != null ? Owner.CardsOnHand.Contains(this) : false; }
        }

        public bool IsOnBattlefield
        {
            get { return Owner != null ? Owner.CardsOnBattlefield.Contains(this) : false; }
        }

        public bool IsHero
        {
            get { return Owner != null ? Owner.Hero == this : false; }
        }
    }
}
