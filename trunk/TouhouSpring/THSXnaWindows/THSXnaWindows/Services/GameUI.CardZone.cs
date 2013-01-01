using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        public class CardZone
        {
            private float m_span;
            private float[] m_intervals;
            private int[] m_intervalReductionLevels;
            private int m_lastFocusIndex;

            public string ZoneId
            {
                get; private set;
            }

            public UI.EventDispatcher Container
            {
                get; private set;
            }

            public bool EnableDepth
            {
                get; private set;
            }

            public CardZone(Style.IStyleContainer style)
            {
                if (style == null)
                {
                    throw new ArgumentNullException("style");
                }

                ZoneId = style.Id.Substring(style.Id.LastIndexOf('.') + 1);
                m_lastFocusIndex = -1;
                Container = style.Target;

                for (var s = style.Parent; s != null; s = s.Parent)
                {
                    if (s.Id == "World3D")
                    {
                        EnableDepth = true;
                        break;
                    }
                }

                var zoneElement = style.Definition.Element("Zone");
                if (zoneElement == null)
                {
                    return;
                }

                var span = zoneElement.Attribute("Span");
                if (span != null)
                {
                    Single.TryParse(span.Value, out m_span);
                }

                var intervals = zoneElement.Attribute("Intervals");
                var intervalReductionLevels = zoneElement.Attribute("IntervalReductionLevels");
                if (intervals != null && intervalReductionLevels != null)
                {
                    var amounts = intervals.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var levels = intervalReductionLevels.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (amounts.Length != levels.Length)
                    {
                        throw new InvalidDataException();
                    }

                    m_intervals = new float[amounts.Length];
                    m_intervalReductionLevels = new int[amounts.Length];
                    for (int i = 0; i < amounts.Length; ++i)
                    {
                        m_intervals[i] = Single.Parse(amounts[i]);
                        m_intervalReductionLevels[i] = Int32.Parse(levels[i]);
                    }
                }
            }

            public void Rearrange()
            {
                Debug.Assert(Container.Listeners.All(l => l is UI.CardControl));
                var locationAnims = Container.Listeners.Cast<UI.CardControl>().Select(cc => cc.GetAddin<UI.CardControlAddins.LocationAnimation>()).ToArray();
                var sortedLocationAnims = locationAnims.OrderBy(la => la.NextLocation.m_thisIndex).ToArray();

                bool inTransition = locationAnims.Any(la => la.InTransition);
                m_lastFocusIndex = inTransition ? m_lastFocusIndex : sortedLocationAnims.FindIndex(la => la.Control.MouseTracked.MouseEntered);

                Container.Listeners.Clear();
                sortedLocationAnims.ForEach(la =>
                {
                    la.NextLocation.m_thisIndex = Container.Listeners.Count;
                    la.NextLocation.m_focusIndex = m_lastFocusIndex;
                    la.NextLocation.m_numCards = sortedLocationAnims.Length;
                    Container.Listeners.Add(la.Control);
                });
            }

            public Matrix ResolveLocationTransform(UI.CardControl control, int thisIndex)
            {
                if (m_intervalReductionLevels == null)
                {
                    return Matrix.Identity;
                }

                int numCards = Container.Listeners.Count;

                int irIndex;
                for (irIndex = m_intervalReductionLevels.Length - 1; irIndex >= 0; --irIndex)
                {
                    if (numCards >= m_intervalReductionLevels[irIndex])
                    {
                        break;
                    }
                }
                var interval = m_intervals[irIndex];

                var zoneOffset = m_span == 0f ? 0f : (m_span - numCards * interval) / 2;

                var zOffset = 0f;
                if (EnableDepth && irIndex > 0)
                {
                    zOffset = thisIndex * 0.005f;
                }

                var xOffset = 0f;
                if (m_lastFocusIndex != -1 && irIndex > 0)
                {
                    xOffset = thisIndex < m_lastFocusIndex
                              ? interval - m_intervals[0]
                              : thisIndex > m_lastFocusIndex
                                ? m_intervals[0] - interval
                                : 0f;
                }

                return MatrixHelper.Translate(zoneOffset + thisIndex * interval + xOffset, 0, zOffset);
            }
        }

        private struct PlayerZones
        {
            public CardZone m_library;
            public CardZone m_hand;
            public CardZone m_sacrifice;
            public CardZone m_battlefield;
            public CardZone m_hero;
            public CardZone m_assists;
        }

        private PlayerZones[] m_playerZones;
        private CardZone m_zoomedInZone;

        private void InitializeCardZones()
        {
            m_playerZones = new PlayerZones[Game.Players.Count];
            for (int i = 0; i < Game.Players.Count; ++i)
            {
                var pid = "Game.Player" + i.ToString();
                m_playerZones[i].m_library = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Library"]);
                m_playerZones[i].m_hand = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Hand"]);
                m_playerZones[i].m_sacrifice = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Sacrifice"]);
                m_playerZones[i].m_battlefield = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Battlefield"]);
                m_playerZones[i].m_hero = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Hero"]);
                m_playerZones[i].m_assists = new CardZone(InGameUIPage.Style.ChildIds[pid + ".Assists"]);
            }

            m_zoomedInZone = new CardZone(InGameUIPage.Style.ChildIds["ZoomedIn"]);
        }

        private void UpdateCardLocations()
        {
            foreach (var cc in m_cardControls)
            {
                var card = cc.Card;
                var locationAnim = cc.GetAddin<UI.CardControlAddins.LocationAnimation>();

                var pid = Game.Players.IndexOf(card.Owner);

                if (cc == ZoomedInCard)
                {
                    locationAnim.SetNextLocation(m_zoomedInZone, 0);
                }
                else if (card.Owner.CardsOnBattlefield.Contains(card))
                {
                    if (card.IsHero)
                    {
                        locationAnim.SetNextLocation(m_playerZones[pid].m_hero, 0);
                    }
                    else
                    {
                        locationAnim.SetNextLocation(m_playerZones[pid].m_battlefield, card.Owner.CardsOnBattlefield.IndexOf(card));
                    }
                }
                else if (card.Owner.CardsSacrificed.Contains(card))
                {
                    locationAnim.SetNextLocation(m_playerZones[pid].m_sacrifice, card.Owner.CardsSacrificed.IndexOf(card));
                }
                else if (card.Owner.CardsOnHand.Contains(card) || card.Owner.Hero == card)
                {
                    locationAnim.SetNextLocation(m_playerZones[pid].m_hand, card.IsHero ? 0 : card.Owner.CardsOnHand.IndexOf(card) + 1);
                }
                else if (card.Owner.Assists.Contains(card))
                {
                    locationAnim.SetNextLocation(m_playerZones[pid].m_assists, card.Owner.Assists.IndexOf(card));
                }
                // TODO: graveyard
                else
                {
                    throw new InvalidOperationException();
                    //locationAnimation.NextLocation.m_zone = m_offboardZoneInfo;
                    //locationAnimation.NextLocation.m_thisIndex = 0;
                }
            }

            foreach (var pzi in m_playerZones)
            {
                pzi.m_hand.Rearrange();
                pzi.m_battlefield.Rearrange();
                pzi.m_sacrifice.Rearrange();
            }
        }
    }
}
