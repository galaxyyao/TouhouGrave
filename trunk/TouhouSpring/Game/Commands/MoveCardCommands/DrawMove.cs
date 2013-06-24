using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DrawMove : BaseCommand, IMoveCard
    {
        private Zone m_fromZone;
        private Zone m_toZone;

        public CardInstance CardDrawn
        {
            get { return Subject; }
        }

        public Player Player
        {
            get; private set;
        }

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

        public DrawMove(Player player, int toZone)
            : this(player, toZone, null)
        { }

        public DrawMove(Player player, int toZone, ICause cause)
            : base(cause)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            m_fromZone = player.m_zones.GetZone(SystemZone.Library);
            m_toZone = player.m_zones.GetZone(toZone);

            Subject = null;
            Player = player;
        }

        internal override void ValidateOnIssue()
        {
            if (!ValidateOnRun())
            {
                FailValidation("Player can't draw card to Zone {0}.", ToZone);
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject == null
                   && FromZoneType == ZoneType.Library
                   && m_fromZone.CardModels != null
                   && ToZoneType != ZoneType.Library
                   && m_toZone.CardInstances != null;
        }

        internal override void RunMain()
        {
            var cardModel = m_fromZone.CardModels[m_fromZone.CardModels.Count - 1];
            m_fromZone.CardModels.RemoveAt(m_fromZone.CardModels.Count - 1);

            Subject = new CardInstance(cardModel, Player);
            m_toZone.CardInstances.Add(Subject);
            Subject.Zone = ToZone;

            if (m_toZone.Type == ZoneType.OnBattlefield)
            {
                Context.Game.SubscribeCardToCommands(Subject);
            }
        }
    }
}
