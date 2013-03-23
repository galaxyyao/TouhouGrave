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

            public virtual Matrix ResolveLocationTransform(UI.CardControl control, int thisIndex)
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

        public class GraveyardZone : CardZone
        {
            public GraveyardZone(Style.IStyleContainer style)
                : base(style)
            { }

            public override Matrix ResolveLocationTransform(UI.CardControl control, int thisIndex)
            {
                // thisIndex is the height of the pile (nextCounter) (see PutToGraveyard())
                Matrix mat = base.ResolveLocationTransform(control, thisIndex)
                    // TODO: no hard coding...
                    * Matrix.CreateTranslation(0, 0, 0.0048f * UI.CardControlAddins.Pile.CardThickness * thisIndex);
                return mat;
            }
        }

        public class ZoomedInZone : CardZone
        {
            public ZoomedInZone(Style.IStyleContainer style)
                : base(style)
            { }

            public override Matrix ResolveLocationTransform(UI.CardControl control, int thisIndex)
            {
                Matrix mat = base.ResolveLocationTransform(control, thisIndex);
                if (!GameApp.Service<GameUI>().ShallPlayerBeRevealed(control.Card.Owner))
                {
                    mat = Matrix.CreateTranslation(-0.5f, control.Region.Height / control.Region.Width * 0.5f, 0)
                          * Matrix.CreateRotationZ(MathHelper.Pi)
                          * Matrix.CreateTranslation(0.5f, -control.Region.Height / control.Region.Width * 0.5f, 0)
                          * mat;
                }
                return mat;
            }
        }

        private PlayerZones[] m_playerZones;
        private CardZone m_actingLocalPlayerHandZone;
        private CardZone m_zoomedInZone;

        private void InitializeCardZones()
        {
            var world3D = InGameUIPage.Style.ChildIds["World3D"];

            m_playerZones = new PlayerZones[Game.Players.Count];
            for (int i = 0; i < Game.Players.Count; ++i)
            {
                m_playerZones[i] = new PlayerZones(Game.Players[i], world3D, GameApp.Service<Styler>().GetPlayerZonesStyle());
            }

            m_actingLocalPlayerHandZone = new CardZone(InGameUIPage.Style.ChildIds["Game.ActingLocalPlayer.Hand"]);
            m_zoomedInZone = new ZoomedInZone(InGameUIPage.Style.ChildIds["ZoomedIn"]);
        }

        private void UpdateCardZones()
        {
            foreach (var pz in m_playerZones)
            {
                pz.UIStyle.Apply();
            }

            for (int i = 0; i < m_playerZones.Length; ++i)
            {
                var pzTransform = m_playerZones[i].UIRoot as UI.ITransformNode;
                pzTransform.Transform = ShallPlayerBeRevealed(Game.Players[i]) ? Matrix.Identity : Matrix.CreateRotationZ(MathHelper.Pi);
            }
        }

        private void UpdateCardLocations()
        {
            foreach (var cc in m_cardControls)
            {
                var card = cc.Card;
                var locationAnim = cc.GetAddin<UI.CardControlAddins.LocationAnimation>();
                var newDesignName = "Normal";

                var pid = card.Owner.Index;

                if (cc == ZoomedInCard)
                {
                    locationAnim.SetNextLocation(m_zoomedInZone, 0);
                    newDesignName = "Full";
                }
                else if (card.Owner.CardsOnBattlefield.Contains(card))
                {
                    if (card.IsHero)
                    {
                        locationAnim.SetNextLocation(m_playerZones[pid].Hero, 0);
                    }
                    else
                    {
                        locationAnim.SetNextLocation(m_playerZones[pid].Battlefield, card.Owner.CardsOnBattlefield.IndexOf(card));
                    }
                    newDesignName = "NoResource";
                }
                else if (card.Owner.CardsSacrificed.Contains(card))
                {
                    locationAnim.SetNextLocation(m_playerZones[pid].Sacrifice, card.Owner.CardsSacrificed.IndexOf(card));
                    newDesignName = "NoResource";
                }
                else if (card.Owner.CardsOnHand.Contains(card) || card.Owner.Hero == card)
                {
                    if (ShallPlayerBeRevealed(card.Owner))
                    {
                        locationAnim.SetNextLocation(m_actingLocalPlayerHandZone, card.IsHero ? 0 : card.Owner.CardsOnHand.IndexOf(card) + 1);
                    }
                    else
                    {
                        locationAnim.SetNextLocation(m_playerZones[pid].Hand, card.IsHero ? 0 : card.Owner.CardsOnHand.IndexOf(card) + 1);
                    }
                }
                else if (card.Owner.Assists.Contains(card))
                {
                    locationAnim.SetNextLocation(m_playerZones[pid].Assists, card.Owner.Assists.IndexOf(card));
                    newDesignName = "NoResource";
                }

                cc.SetCardDesign(newDesignName);
            }

            foreach (var pz in m_playerZones)
            {
                pz.Hand.Rearrange();
                pz.Battlefield.Rearrange();
                pz.Sacrifice.Rearrange();
            }
            m_actingLocalPlayerHandZone.Rearrange();
        }
    }
}
