using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DrawMove<TToZone> : BaseCommand,
        IMoveCard<Library, TToZone>
        where TToZone : IZoneToken, new()
    {
        private static TToZone s_toZone = new TToZone();

        public CardInstance Subject { get; private set; }
        public int FromZone { get { return SystemZone.Library; } }
        public int ToZone { get { return s_toZone.Zone; } }

        public Player Player
        {
            get; private set;
        }

        public DrawMove(Player player)
            : this(player, null)
        { }

        public DrawMove(Player player, ICause cause)
            : base(cause)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override bool ValidateOnRun()
        {
            return true;
        }

        internal override void RunMain()
        {
            var cardModel = Player.Library.RemoveFromTop();
            Debug.Assert(cardModel != null);
            Subject = new CardInstance(cardModel, Player);
            s_toZone.Add(Subject);
            Subject.Zone = s_toZone.Zone;
        }
    }
}
