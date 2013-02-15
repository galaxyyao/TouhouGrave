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

        public Int32 Guid
        {
            get; private set;
        }

        public bool IsOnBattlefield
        {
            get { return Owner.CardsOnBattlefield.Contains(this); }
        }

        public bool IsHero
        {
            get { return Owner.Hero == this; }
        }

        public bool IsAssist
        {
            get { return Owner.Assists.Contains(this); }
        }

        public bool IsDestroyed
        {
            get; internal set; // set by Kill command
        }

        internal BaseCard(ICardModel model, Player owner)
            : this(model, owner, true)
        { }

        private BaseCard()
        {
            Behaviors = new Behaviors.BehaviorList(this);
        }

        private BaseCard(ICardModel model, Player owner, bool hasGuid) : this()
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

        public static BaseCard CreateDummyCard(Player owner)
        {
            CardModel dummyModel = new CardModel
            {
                Behaviors = new List<Behaviors.IBehaviorModel>()
            };
            return new BaseCard(dummyModel, owner, false);
        }
    }
}
