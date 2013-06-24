using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class KillMove : BaseCommand, IMoveCard
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

        public KillMove(CardInstance subject)
            : this(subject, null)
        { }

        public KillMove(CardInstance subject, ICause cause)
            : base(cause)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            m_fromZone = subject.Owner.m_zones.GetZone(subject.Zone);
            m_toZone = subject.Owner.m_zones.GetZone(SystemZone.Graveyard);

            Subject = subject;
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
                   && m_fromZone.Type != ZoneType.Library
                   && m_fromZone.CardInstances != null
                   && m_toZone.Type == ZoneType.Library
                   && m_toZone.CardModels != null
                   && m_fromZone.CardInstances.Contains(Subject);
        }

        internal override void RunMain()
        {
            m_fromZone.CardInstances.Remove(Subject);
            m_toZone.CardModels.Add(Subject.Model);
            Subject.Zone = ToZone;
            if (m_fromZone.Type == ZoneType.OnBattlefield)
            {
                Context.Game.UnsubscribeCardFromCommands(Subject);
            }
        }
    }
}
