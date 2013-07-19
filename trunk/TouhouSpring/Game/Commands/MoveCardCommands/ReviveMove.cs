using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class ReviveMove : BaseCommand, IMoveCard
    {
        private Zone m_fromZone;
        private Zone m_toZone;

        public ICardModel CardToRevive
        {
            get; private set;
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

        public ReviveMove(Player player, ICardModel cardToRevive, int fromZone, int toZone)
            : this(player, cardToRevive, fromZone, toZone, null)
        { }

        public ReviveMove(Player player, ICardModel cardToRevive, int fromZone, int toZone, ICause cause)
            : base(cause)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (cardToRevive == null)
            {
                throw new ArgumentNullException("cardToRevive");
            }

            m_fromZone = player.m_zones.GetZone(fromZone);
            m_toZone = player.m_zones.GetZone(toZone);

            Subject = null;
            Player = player;
            CardToRevive = cardToRevive;
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
                FailValidation("Player can't draw card to Zone {0}.", ToZone);
            }
        }

        internal override bool ValidateOnRun()
        {
            return Subject == null
                   && FromZoneType == ZoneType.Library
                   && m_fromZone.CardModels != null
                   && m_fromZone.CardModels.Contains(CardToRevive)
                   && ToZoneType != ZoneType.Library
                   && m_toZone.CardInstances != null;
        }

        internal override void RunMain()
        {
            var index = m_fromZone.CardModels.IndexOf(CardToRevive);
            m_fromZone.CardModels.RemoveAt(index);

            Subject = new CardInstance(CardToRevive, Player);
            m_toZone.CardInstances.Add(Subject);
            Subject.Zone = ToZone;

            if (m_toZone.Type == ZoneType.OnBattlefield)
            {
                Context.Game.SubscribeCardToCommands(Subject);
            }
        }
    }
}
