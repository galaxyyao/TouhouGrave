using System;
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
            cardControl.Addins.Add(new UI.CardControlAddins.SelectedAnimation(cardControl));
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

        public bool IsCardSelected(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }
            return SelectCards_ShouldBeHighlighted(card)
                   || TacticalPhase_ShouldBeHighlighted(card)
                   || BlockPhase_ShouldBeHighlighted(card);
        }

        public bool IsSelectable(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            if (InteractionObject is Interactions.TacticalPhase)
            {
                var io = (Interactions.TacticalPhase)InteractionObject;
                return io.FromSet.Contains(card) || io.ComputeCastFromSet().Contains(card);
            }
            else if (InteractionObject is Interactions.BlockPhase)
            {
                var io = (Interactions.BlockPhase)InteractionObject;
                return io.BlockerCandidates.Contains(card)
                       || io.PlayableCandidates.Contains(card)
                       || io.DeclaredAttackers.Contains(card);
            }
            else if (InteractionObject is Interactions.SelectCards)
            {
                var io = (Interactions.SelectCards)InteractionObject;
                return io.FromSet.Contains(card);
            }
            else
            {
                return false;
            }
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
                m_intervalReductionAmount = new float[] { 1.1f, 0.7f, 0.5f }
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
            var card = control.Card;

            var io = InteractionObject;
            if (io is Interactions.TacticalPhase)
            {
                TacticalPhaes_OnCardClicked(control, io as Interactions.TacticalPhase);
            }
            else if (io is Interactions.SelectCards)
            {
                SelectCards_OnCardClicked(control, io as Interactions.SelectCards);
            }
            else if (io is Interactions.BlockPhase)
            {
                BlockPhase_OnCardClicked(control, io as Interactions.BlockPhase);
            }
        }

        internal void OnSpellClicked(UI.CardControl control, Behaviors.ICastableSpell spell)
        {
            Debug.Assert(control.Card == spell.Host);
            var card = control.Card;

            var io = InteractionObject;
            if (io is Interactions.TacticalPhase && Game.PlayerPlayer.CardsOnBattlefield.Contains(card))
            {
                GameApp.Service<ModalDialog>().Show(String.Format("Cast {0}?", spell.Model.GetName()), ModalDialog.Button.Yes | ModalDialog.Button.Cancel, btn =>
                {
                    if (btn == ModalDialog.Button.Yes)
                    {
                        InteractionObject = null;
                        (io as Interactions.TacticalPhase).Respond(spell);
                    }
                });
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
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_zoomedInZoneInfo,
                        m_numCards = 1,
                        m_thisIndex = -1,
                        m_focusIndex = -1
                    };
                }
                else if (Game.Players[0].CardsOnHand.Contains(card))
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_playerHandZoneInfo,
                        m_numCards = Game.Players[0].CardsOnHand.Count,
                        m_thisIndex = Game.Players[0].CardsOnHand.IndexOf(card),
                        m_focusIndex = -1
                    };
                }
                else if (Game.Players[1].CardsOnHand.Contains(card))
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_opponentHandZoneInfo,
                        m_numCards = Game.Players[1].CardsOnHand.Count,
                        m_thisIndex = Game.Players[1].CardsOnHand.IndexOf(card),
                        m_focusIndex = -1
                    };

                    cc.EnableDepth = true;
                }
                else if (m_interactionObject is Interactions.BlockPhase && (m_interactionObject as Interactions.BlockPhase).DeclaredAttackers.Contains(card))
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = (m_interactionObject as Interactions.BlockPhase).Controller == Game.Controllers[0] ? m_opponentFormationZoneInfo : m_playerFormationZoneInfo,
                        m_numCards = (m_interactionObject as Interactions.BlockPhase).DeclaredAttackers.Count,
                        m_thisIndex = (m_interactionObject as Interactions.BlockPhase).DeclaredAttackers.IndexOf(card),
                        m_focusIndex = -1
                    };

                    cc.EnableDepth = true;
                }
                else if (m_interactionObject is Interactions.BlockPhase && m_declaredBlockers.Any(b => b.Contains(cc)))
                {
                    var blockers = m_declaredBlockers.SelectMany(b => b.Where(c => c != null));
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = (m_interactionObject as Interactions.BlockPhase).Controller == Game.Controllers[0] ? m_playerFormationZoneInfo : m_opponentFormationZoneInfo,
                        
                        m_numCards = blockers.Count(),
                        m_thisIndex = blockers.FindIndex(c => c == cc),
                        m_focusIndex = -1
                    };

                    cc.EnableDepth = true;
                }
                else if (Game.Players[0].CardsOnBattlefield.Contains(card))
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_playerBattlefieldZoneInfo,
                        m_numCards = Game.Players[0].CardsOnBattlefield.Count,
                        m_thisIndex = Game.Players[0].CardsOnBattlefield.IndexOf(card),
                        m_focusIndex = -1
                    };

                    cc.EnableDepth = true;
                }
                else if (Game.Players[1].CardsOnBattlefield.Contains(card))
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_opponentBattlefieldZoneInfo,
                        m_numCards = Game.Players[1].CardsOnBattlefield.Count,
                        m_thisIndex = Game.Players[1].CardsOnBattlefield.IndexOf(card),
                        m_focusIndex = -1
                    };

                    cc.EnableDepth = true;
                }
                // TODO: graveyard
                else if (Game.Players[0].Hero.Host == card)
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_playerHeroZoneInfo,
                        m_numCards = 1,
                        m_thisIndex = -1,
                        m_focusIndex = -1
                    };
                }
                else if (Game.Players[1].Hero.Host == card)
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_opponentHeroZoneInfo,
                        m_numCards = 1,
                        m_thisIndex = -1,
                        m_focusIndex = -1
                    };
                }
                else
                {
                    locationAnimation.NextLocation = new LocationAnimation.LocationParameter
                    {
                        m_zone = m_offboardZoneInfo,
                        m_numCards = 0,
                        m_thisIndex = -1,
                        m_focusIndex = -1
                    };
                }

                locationAnimation.OnLocationSet();
            }

            RearrangeZone(InGameUIPage.Style.ChildIds["PlayerHand"].Target);
            RearrangeZone(InGameUIPage.Style.ChildIds["PlayerBattlefield"].Target);
            RearrangeZone(InGameUIPage.Style.ChildIds["PlayerFormation"].Target);
            RearrangeZone(InGameUIPage.Style.ChildIds["OpponentHand"].Target);
            RearrangeZone(InGameUIPage.Style.ChildIds["OpponentBattlefield"].Target);
            RearrangeZone(InGameUIPage.Style.ChildIds["OpponentFormation"].Target);
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

        private void RearrangeZone(UI.EventDispatcher zone)
        {
            Debug.Assert(zone.Listeners.All(l => l is UI.CardControl));
            var cardControls = zone.Listeners.Cast<UI.CardControl>().ToArray();

            zone.Listeners.Clear();
            cardControls.OrderBy(
                cc => (cc.Addins.First(addin => addin is LocationAnimation) as LocationAnimation).NextLocation.m_thisIndex)
                .ForEach(cc => zone.Listeners.Add(cc));

            var focusIndex = cardControls.FindIndex(cc => cc.MouseTracked.MouseEntered);
            cardControls.ForEach(cc => (cc.Addins.First(addin => addin is LocationAnimation) as LocationAnimation).NextLocation.m_focusIndex = focusIndex);
        }
    }
}
