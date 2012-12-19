﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LocationAnimation = TouhouSpring.UI.CardControlAddins.LocationAnimation;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private List<UI.CardControl> m_cardControls = new List<UI.CardControl>();
        private LocationAnimation.ZoneInfo m_zoomedInZoneInfo;
        private LocationAnimation.ZoneInfo m_playerHandZoneInfo;
        private LocationAnimation.ZoneInfo m_opponentHandZoneInfo;
        private LocationAnimation.ZoneInfo m_playerBattlefieldZoneInfo;
        private LocationAnimation.ZoneInfo m_opponentBattlefieldZoneInfo;
        private LocationAnimation.ZoneInfo m_playerFormationZoneInfo;
        private LocationAnimation.ZoneInfo m_opponentFormationZoneInfo;
        private LocationAnimation.ZoneInfo m_playerHeroZoneInfo;
        private LocationAnimation.ZoneInfo m_opponentHeroZoneInfo;
        private LocationAnimation.ZoneInfo m_offboardZoneInfo;

        public void RegisterCard(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }
            else if (m_cardControls.Any(cc => cc.Card == card))
            {
                return;
            }

            var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("Large"), card);
            ccStyle.Initialize();

            var cardControl = ccStyle.TypedTarget;
            //cardControl.Addins.Add(new UI.CardControlAddins.Flip(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.Glow(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.Highlight(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.LocationAnimation(cardControl, Card_ResolveLocationTransform));
            //cardControl.Addins.Add(new UI.CardControlAddins.SelectedAnimation(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.SpellButtons(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.ToneAnimation(cardControl));
            m_cardControls.Add(cardControl);
        }

        public void UnregisterCard(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            if (card.Owner.Hero.Host == card)
            {
                // skip unregistering hero card
                return;
            }

            var index = m_cardControls.FindIndex(cc => cc.Card == card);
            m_cardControls[index].Dispose();
            m_cardControls.RemoveAt(index);
        }

        public bool TryGetCardControl(BaseCard card, out UI.CardControl cardControl)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            cardControl = m_cardControls.FirstOrDefault(cc => cc.Card == card);
            return cardControl != null;
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return ZoomedInCard != cardControl && UIState != null
                   ? UIState.IsCardClickable(cardControl)
                   : false;
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return UIState != null
                   ? UIState.IsCardSelected(cardControl)
                   : false;
        }

        public bool IsCardSelectedForCastSpell(UI.CardControl cardControl)
        {
            return ZoomedInCard != cardControl && UIState != null
                   ? UIState.IsCardSelectedForCastSpell(cardControl)
                   : false;
        }

        public int GetRenderIndex(UI.CardControl cardControl)
        {
            if (cardControl == null)
            {
                throw new ArgumentNullException("cardControl");
            }

            return m_cardControls.IndexOf(cardControl);
        }

        private void InitializeZoneInfos()
        {
            m_zoomedInZoneInfo = new LocationAnimation.ZoneInfo { m_container = InGameUIPage.Style.ChildIds["ZoomedIn"].Target };
            m_playerHandZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["PlayerHand"].Target,
                m_width = 8f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f },
                m_lastFocusIndex = -1
            };
            m_opponentHandZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["OpponentHand"].Target,
                m_width = 10f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
            };
            m_playerFormationZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["PlayerFormation"].Target,
                m_width = 10f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
            };
            m_opponentFormationZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["OpponentFormation"].Target,
                m_width = 10f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
            };
            m_playerBattlefieldZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["PlayerBattlefield"].Target,
                m_width = 10f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
            };
            m_opponentBattlefieldZoneInfo = new LocationAnimation.ZoneInfo
            {
                m_container = InGameUIPage.Style.ChildIds["OpponentBattlefield"].Target,
                m_width = 10f,
                m_intervalReductionLevel = new int[] { 0, 8, 12 },
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
            };
            m_playerHeroZoneInfo = new LocationAnimation.ZoneInfo { m_container = InGameUIPage.Style.ChildIds["PlayerHero"].Target };
            m_opponentHeroZoneInfo = new LocationAnimation.ZoneInfo { m_container = InGameUIPage.Style.ChildIds["OpponentHero"].Target };
            m_offboardZoneInfo = new LocationAnimation.ZoneInfo { m_container = InGameUIPage };
        }

        private void UnregisterAllCards()
        {
            m_cardControls.ForEach(cc => cc.Dispose());
            m_cardControls.Clear();
        }

        private void UpdateCardControls(float deltaTime)
        {
            if (Game == null)
            {
                return;
            }

            UpdateCardLocations();
            m_cardControls.ForEach(cc => cc.Update(deltaTime));
        }

        internal void OnCardClicked(UI.CardControl control)
        {
            Debug.Assert(control != null && control.Card != null);
            if (UIState != null)
            {
                UIState.OnCardClicked(control);
            }
        }

        internal void OnSpellClicked(UI.CardControl control, Behaviors.ICastableSpell spell)
        {
            Debug.Assert(control.Card == spell.Host);
            if (UIState != null)
            {
                UIState.OnSpellClicked(control, spell);
            }
        }

        private void UpdateCardLocations()
        {
            foreach (var cc in m_cardControls)
            {
                var card = cc.Card;
                var locationAnimation = cc.Addins.First(addin => addin is LocationAnimation) as LocationAnimation;

                cc.EnableDepth = false;

                if (cc == ZoomedInCard)
                {
                    locationAnimation.NextLocation.m_zone = m_zoomedInZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = 0;
                }
                else if (Game.Players[0].CardsOnHand.Contains(card))
                {
                    locationAnimation.NextLocation.m_zone = m_playerHandZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = Game.Players[0].CardsOnHand.IndexOf(card);
                }
                else if (Game.Players[1].CardsOnHand.Contains(card))
                {
                    locationAnimation.NextLocation.m_zone = m_opponentHandZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = Game.Players[1].CardsOnHand.IndexOf(card);

                    cc.EnableDepth = true;
                }
                else if (UIState is UIStates.DeclareAttackers
                    && (UIState as UIStates.DeclareAttackers).Selection.Contains(card))
                {
                    var declareAttackers = UIState as UIStates.DeclareAttackers;
                    locationAnimation.NextLocation.m_zone = declareAttackers.Player == Game.Players[0] ? m_playerFormationZoneInfo : m_opponentFormationZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = declareAttackers.Selection.IndexOf(card);

                    cc.EnableDepth = true;
                }
                else if (UIState is UIStates.BlockPhase
                    && (UIState as UIStates.BlockPhase).DeclaredAttackers.Contains(card))
                {
                    var blockPhase = UIState as UIStates.BlockPhase;
                    locationAnimation.NextLocation.m_zone = blockPhase.Player == Game.Players[0] ? m_opponentFormationZoneInfo : m_playerFormationZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = blockPhase.DeclaredAttackers.IndexOf(card);

                    cc.EnableDepth = true;
                }
                else if (UIState is UIStates.BlockPhase
                    && (UIState as UIStates.BlockPhase).DeclaredBlockers.Any(b => b.Contains(cc)))
                {
                    var blockPhase = UIState as UIStates.BlockPhase;
                    var blockers = blockPhase.DeclaredBlockers.SelectMany(b => b.Where(c => c != null));
                    locationAnimation.NextLocation.m_zone = blockPhase.Player == Game.Players[0] ? m_playerFormationZoneInfo : m_opponentFormationZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = blockers.FindIndex(c => c == cc);

                    cc.EnableDepth = true;
                }
                else if (Game.Players[0].Hero.Host == card)
                {
                    locationAnimation.NextLocation.m_zone = m_playerHeroZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = 0;
                }
                else if (Game.Players[1].Hero.Host == card)
                {
                    locationAnimation.NextLocation.m_zone = m_opponentHeroZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = 0;
                }
                else if (Game.Players[0].CardsOnBattlefield.Contains(card))
                {
                    Debug.Assert(Game.Players[0].CardsOnBattlefield.Count > 0);
                    Debug.Assert(Game.Players[0].CardsOnBattlefield[0].Behaviors.Has<Behaviors.Hero>());
                    locationAnimation.NextLocation.m_zone = m_playerBattlefieldZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = Game.Players[0].CardsOnBattlefield.IndexOf(card) - 1;

                    cc.EnableDepth = true;
                }
                else if (Game.Players[1].CardsOnBattlefield.Contains(card))
                {
                    Debug.Assert(Game.Players[1].CardsOnBattlefield.Count > 0);
                    Debug.Assert(Game.Players[1].CardsOnBattlefield[0].Behaviors.Has<Behaviors.Hero>());
                    locationAnimation.NextLocation.m_zone = m_opponentBattlefieldZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = Game.Players[1].CardsOnBattlefield.IndexOf(card) - 1;

                    cc.EnableDepth = true;
                }
                // TODO: graveyard
                else
                {
                    locationAnimation.NextLocation.m_zone = m_offboardZoneInfo;
                    locationAnimation.NextLocation.m_thisIndex = 0;
                }

                locationAnimation.OnLocationSet();
            }

            RearrangeZone(m_playerHandZoneInfo);
            RearrangeZone(m_playerBattlefieldZoneInfo);
            RearrangeZone(m_playerFormationZoneInfo);
            RearrangeZone(m_opponentHandZoneInfo);
            RearrangeZone(m_opponentBattlefieldZoneInfo);
            RearrangeZone(m_opponentFormationZoneInfo);
        }

        private void RearrangeZone(LocationAnimation.ZoneInfo zone)
        {
            var zoneContainer = zone.m_container;
            Debug.Assert(zoneContainer.Listeners.All(l => l is UI.CardControl));
            var locationAnims = zoneContainer.Listeners.Cast<UI.CardControl>().Select(cc => (cc.Addins.First(addin => addin is LocationAnimation) as LocationAnimation)).ToArray();
            var sortedLocationAnims = locationAnims.OrderBy(la => la.NextLocation.m_thisIndex).ToArray();

            bool inTransition = locationAnims.Any(la => la.InTransition);
            zone.m_lastFocusIndex = inTransition ? zone.m_lastFocusIndex : sortedLocationAnims.FindIndex(la => la.Control.MouseTracked.MouseEntered);

            zoneContainer.Listeners.Clear();
            sortedLocationAnims.ForEach(la =>
            {
                la.NextLocation.m_thisIndex = zoneContainer.Listeners.Count;
                la.NextLocation.m_focusIndex = zone.m_lastFocusIndex;
                la.NextLocation.m_numCards = sortedLocationAnims.Length;
                zoneContainer.Listeners.Add(la.Control);
            });
        }

        private Matrix Card_ResolveLocationTransform(LocationAnimation.LocationParameter location, UI.CardControl control)
        {
            if (location.m_zone.m_width == 0)
            {
                return Matrix.Identity;
            }

            int irIndex;
            for (irIndex = location.m_zone.m_intervalReductionLevel.Length - 1; irIndex >= 0; --irIndex)
            {
                if (location.m_numCards >= location.m_zone.m_intervalReductionLevel[irIndex])
                {
                    break;
                }
            }
            var interval = location.m_zone.m_intervalReductionAmount[irIndex];

            var zoneOffset = (location.m_zone.m_width - location.m_numCards * interval) / 2;

            var zOffset = 0f;
            if (control.EnableDepth && irIndex > 0)
            {
                zOffset = location.m_thisIndex * 0.005f;
            }

            var xOffset = 0f;
            if (location.m_focusIndex != -1 && irIndex > 0)
            {
                xOffset = location.m_thisIndex < location.m_focusIndex
                          ? interval - location.m_zone.m_intervalReductionAmount[0]
                          : location.m_thisIndex > location.m_focusIndex
                            ? location.m_zone.m_intervalReductionAmount[0] - interval
                            : 0f;
            }

            return MatrixHelper.Translate(zoneOffset + location.m_thisIndex * interval + xOffset, 0, zOffset);
        }
    }
}
