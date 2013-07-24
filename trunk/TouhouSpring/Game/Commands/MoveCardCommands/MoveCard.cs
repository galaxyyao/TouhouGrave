using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class MoveCard : BaseCommand, IMoveCard
    {
        private Zone m_fromZone;
        private Zone m_toZone;

        public CardInstance Subject
        {
            get; private set;
        }

        public int FromZone
        {
            get { return m_fromZone.Id; }
        }

        public ZoneType FromZoneType
        {
            get { return m_fromZone.Type; }
        }

        public int ToZone
        {
            get { return m_toZone.Id; }
        }

        public ZoneType ToZoneType
        {
            get { return m_toZone.Type; }
        }

        public MoveCard(CardInstance subject, int toZone)
            : this(subject, toZone, null)
        { }

        public MoveCard(CardInstance subject, int toZone, ICause cause)
            : base(cause)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            m_fromZone = subject.Owner.m_zones.GetZone(subject.Zone);
            m_toZone = subject.Owner.m_zones.GetZone(toZone);

            Subject = subject;
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
                FailValidation("Card {0} can't be moved from Zone {1} to Zone {2}.", Subject.Model.Name, FromZone, ToZone);
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject.Zone == FromZone
                   && m_fromZone != m_toZone
                   && m_fromZone.Type != ZoneType.Library
                   && m_fromZone.CardInstances != null
                   && m_fromZone.CardInstances.Contains(Subject)
                   && (m_toZone.CardInstances == null || !m_toZone.CardInstances.Contains(Subject));
        }

        internal override void RunMain()
        {
            m_fromZone.CardInstances.Remove(Subject);
            if (m_toZone.CardInstances != null)
            {
                m_toZone.CardInstances.Add(Subject);
            }
            else
            {
                m_toZone.CardModels.Add(Subject.Model);
            }
            Subject.Zone = ToZone;
            if (m_fromZone.Type != ZoneType.OnBattlefield && m_toZone.Type == ZoneType.OnBattlefield)
            {
                Context.Game.SubscribeCardToCommands(Subject);
            }
            else if (m_fromZone.Type == ZoneType.OnBattlefield && m_toZone.Type != ZoneType.OnBattlefield)
            {
                Subject.Reset(null);
                Context.Game.UnsubscribeCardFromCommands(Subject);
            }
        }
    }

    public class InitiativeMoveCard : MoveCard, IInitiativeMoveCard
    {
        public Player Initiator
        {
            get { return Subject.Owner; }
        }

        public InitiativeMoveCard(CardInstance subject, int toZone)
            : base(subject, toZone)
        { }

        public InitiativeMoveCard(CardInstance subject, int toZone, ICause cause)
            : base(subject, toZone, cause)
        { }
    }

    public class PlayCard : InitiativeMoveCard
    {
        public PlayCard(CardInstance subject, int toZone, Game cause)
            : base(subject, toZone, cause)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }
            else if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }
            else if (FromZone != SystemZone.Hand
                || ToZoneType != ZoneType.OnBattlefield)
            {
                throw new InvalidOperationException("Invalid card to be played.");
            }
        }
    }
}
