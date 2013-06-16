using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public abstract class MoveCardBase : BaseCommand
    {
        public CardInstance Subject
        {
            get; protected set;
        }

        public abstract int FromZone
        {
            get;
        }

        public abstract int ToZone
        {
            get;
        }

        protected MoveCardBase(CardInstance subject, ICause cause)
            : base(cause)
        {
            Subject = subject;
        }
    }

    public class MoveCard<TFromZone, TToZone> : MoveCardBase
        where TFromZone : IZoneToken
        where TToZone : IZoneToken
    {
        private static TFromZone s_fromZone;
        private static TToZone s_toZone;

        public override int FromZone
        {
            get { return s_fromZone.Zone; }
        }

        public override int ToZone
        {
            get { return s_toZone.Zone; }
        }

        public MoveCard(CardInstance subject)
            : this(subject, null)
        { }

        public MoveCard(CardInstance subject, ICause cause)
            : base(subject, cause)
        { }

        internal override void ValidateOnIssue()
        {
            Validate(Subject);
            if (Subject.Zone != s_fromZone.Zone || !s_fromZone.ValidateRemove(Subject))
            {
                FailValidation("Card can't be moved from the specified zone.");
            }
            if (!s_toZone.ValidateAdd(Subject))
            {
                FailValidation("Card can't be moved to the specified zone.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject.Zone == s_fromZone.Zone && s_fromZone.ValidateRemove(Subject) && s_toZone.ValidateAdd(Subject);
        }

        internal override void RunMain()
        {
            s_fromZone.Remove(Subject);
            s_toZone.Add(Subject);
            Subject.Zone = s_toZone.Zone;
        }
    }

    public class InitiativeMoveCard<TFromZone, TToZone> : MoveCard<TFromZone, TToZone>,
        IInitiativeCommand
        where TFromZone : IZoneToken
        where TToZone : IZoneToken
    {
        public Player Initiator
        {
            get { return Subject.Owner; }
        }

        public InitiativeMoveCard(CardInstance subject)
            : base(subject)
        { }
    }
}
