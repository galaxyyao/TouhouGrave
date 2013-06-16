using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class KillMove<TFromZone> : MoveCardBase
        where TFromZone : IZoneToken
    {
        private static TFromZone s_fromZone;

        public override int FromZone { get { return s_fromZone.Zone; } }
        public override int ToZone { get { return SystemZone.Graveyard; } }

        public KillMove(CardInstance subject)
            : this(subject, null)
        { }

        public KillMove(CardInstance subject, ICause cause)
            : base(subject, cause)
        { }

        internal override void ValidateOnIssue()
        {
            Validate(Subject);
            if (Subject.Zone != s_fromZone.Zone || !s_fromZone.ValidateRemove(Subject))
            {
                FailValidation("Card can't be moved from the specified zone.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject.Zone == s_fromZone.Zone && s_fromZone.ValidateRemove(Subject);
        }

        internal override void RunMain()
        {
            s_fromZone.Remove(Subject);
            Subject.Owner.Graveyard.AddToTop(Subject.Model);
            Subject.Zone = SystemZone.Graveyard;
        }
    }
}
