using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SummonMove : BaseCommand, IMoveCard
    {
        private Zone m_toZone;

        public CardInstance CardSummoned
        {
            get { return Subject; }
        }

        public Player Owner
        {
            get; private set;
        }

        public ICardModel Model
        {
            get; private set;
        }

        public CardInstance Subject
        {
            get; private set;
        }

        public int FromZone
        {
            get { return SystemZone.Unknown; }
        }

        public ZoneType FromZoneType
        {
            get { return ZoneType.Unknown; }
        }

        public int ToZone
        {
            get { return m_toZone.Id; }
        }

        public ZoneType ToZoneType
        {
            get { return m_toZone.Type; }
        }

        public SummonMove(ICardModel model, Player owner, int toZone)
            : this(model, owner, toZone, null)
        { }

        public SummonMove(ICardModel model, Player owner, int toZone, ICause cause)
            : base(cause)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            else if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            m_toZone = owner.m_zones.GetZone(toZone);

            Subject = null;
            Model = model;
            Owner = owner;
        }

        public void PatchZoneMoveTo(int newToZone)
        {
            CheckPatchable("ToZone");
            m_toZone = Subject.Owner.m_zones.GetZone(newToZone);
            ValidateOnIssue();
        }

        internal override void ValidateOnIssue()
        {
            if (!ValidateOnRun())
            {
                FailValidation("Player can't summon card to Zone {0}.", ToZone);
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject == null
                   && m_toZone.Type != ZoneType.Library
                   && m_toZone.CardInstances != null;
        }

        internal override void RunMain()
        {
            Subject = new CardInstance(Model, Owner);
            m_toZone.CardInstances.Add(Subject);
            Subject.Zone = ToZone;

            if (m_toZone.Type == ZoneType.OnBattlefield)
            {
                Context.Game.SubscribeCardToCommands(Subject);
            }
        }
    }
}
