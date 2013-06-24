using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public static class SystemZone
    {
        public static readonly int Unknown = 0;
        public static readonly int Library = 1000;
        public static readonly int Hand = 1001;
        public static readonly int Sacrifice = 1002;
        public static readonly int Battlefield = 1003;
        public static readonly int Graveyard = 1004;
        public static readonly int Assist = 1005;
    }

    public enum ZoneType
    {
        Unknown,
        Library,
        OnBattlefield,
        OffBattlefield
    }

    public class Zone
    {
        public int Id { get; private set; }
        public ZoneType Type { get; private set; }
        public Player Player { get; private set; }
        public List<CardInstance> CardInstances { get; private set; }
        public List<ICardModel> CardModels { get; private set; }

        internal Zone(int id, ZoneType type, Player player)
        {
            Id = id;
            Type = type;
            Player = player;
            CardInstances = type != ZoneType.Library ? new List<CardInstance>() : null;
            CardModels = type == ZoneType.Library ? new List<ICardModel>() : null;
        }
    }

    class Zones
    {
        private List<Zone> m_zones;

        internal Zones(Dictionary<int, ZoneType> zoneConfig, Player player)
        {
            Debug.Assert(m_zones == null);

            m_zones = new List<Zone>();
            foreach (var kvp in zoneConfig)
            {
                m_zones.Add(new Zone(kvp.Key, kvp.Value, player));
            }
        }

        internal Zone GetZone(int zoneId)
        {
            return m_zones.Find(zone => zone.Id == zoneId);
        }
    }
}
