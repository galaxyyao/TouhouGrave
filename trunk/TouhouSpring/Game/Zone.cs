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

    public enum ZoneVisibility
    {
        VisibleToOwner,
        Visible,
        Invisible
    }

    public struct ZoneConfig
    {
        public int Id;
        public ZoneType Type;
        public ZoneVisibility Visibility;
    }

    public class Zone
    {
        public Player Owner { get; private set; }
        public int Id { get; private set; }
        public ZoneType Type { get; private set; }
        public ZoneVisibility Visibility { get; private set; }
        public List<CardInstance> CardInstances { get; private set; }
        public List<ICardModel> CardModels { get; private set; }

        internal Zone(Player owner, int id, ZoneType type, ZoneVisibility visibility)
        {
            Owner = owner;
            Id = id;
            Type = type;
            Visibility = visibility;
            CardInstances = type != ZoneType.Library ? new List<CardInstance>() : null;
            CardModels = type == ZoneType.Library ? new List<ICardModel>() : null;
        }
    }

    class Zones
    {
        private Zone[] m_zones;

        internal Zones(IIndexable<ZoneConfig> zoneConfigs, Player owner)
        {
            Debug.Assert(m_zones == null);

            m_zones = new Zone[zoneConfigs.Count];
            for (int i = 0; i < zoneConfigs.Count; ++i)
            {
                m_zones[i] = new Zone(owner, zoneConfigs[i].Id, zoneConfigs[i].Type, zoneConfigs[i].Visibility);
            }
        }

        internal Zone GetZone(int zoneId)
        {
            for (int i = 0; i < m_zones.Length; ++i)
            {
                if (m_zones[i].Id == zoneId)
                {
                    return m_zones[i];
                }
            }
            throw new ArgumentException("Zone with the specified Id can't be found.");
        }
    }
}
