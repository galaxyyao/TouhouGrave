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
        private IEnumerable<CardDataManager.ICardData> m_cardData;
        private List<UI.CardControl> m_cardControls = new List<UI.CardControl>();

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            return ZoomedInCard != cardControl && UIState != null
                   ? UIState.IsCardClickable(cardControl)
                   : false;
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return ZoomedInCard != cardControl && UIState != null
                   ? UIState.IsCardSelected(cardControl)
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

        private void UpdateCardControls(float deltaTime)
        {
            var newCards = GameApp.Service<CardDataManager>().Collection;

            for (int i = 0; i < m_cardControls.Count; ++i)
            {
                var cc = m_cardControls[i];
                var data = newCards.FirstOrDefault(cardData => cardData.Guid == cc.CardGuid);
                if (data == null)
                {
                    m_cardControls.RemoveAt(i--);
                    PutToGraveyard(cc);
                    continue;
                }

                cc.CardData = data;
                var locationAnim = cc.GetAddin<UI.CardControlAddins.LocationAnimation>();
                var newDesignName = "Normal";

                if (cc == ZoomedInCard)
                {
                    locationAnim.SetNextLocation(m_zoomedInZone, 0);
                    newDesignName = "Full";
                }
                else if (data.Location == CardDataManager.CardLocation.Battlefield)
                {
                    if (data.IsHero)
                    {
                        locationAnim.SetNextLocation(m_playerZones[data.OwnerPlayerIndex].Hero, 0);
                    }
                    else
                    {
                        locationAnim.SetNextLocation(m_playerZones[data.OwnerPlayerIndex].Battlefield, data.LocationIndex);
                    }
                    newDesignName = "NoResource";
                }
                else if (data.Location == CardDataManager.CardLocation.Sacrifice)
                {
                    locationAnim.SetNextLocation(m_playerZones[data.OwnerPlayerIndex].Sacrifice, data.LocationIndex);
                    newDesignName = "NoResource";
                }
                else if (data.Location == CardDataManager.CardLocation.Hand || data.IsHero)
                {
                    if (ShallPlayerBeRevealed(data.OwnerPlayerIndex))
                    {
                        locationAnim.SetNextLocation(m_actingLocalPlayerHandZone, data.IsHero ? 0 : data.LocationIndex + 1);
                    }
                    else
                    {
                        locationAnim.SetNextLocation(m_playerZones[data.OwnerPlayerIndex].Hand, data.IsHero ? 0 : data.LocationIndex + 1);
                    }
                }
                else if (data.Location == CardDataManager.CardLocation.Assist)
                {
                    locationAnim.SetNextLocation(m_playerZones[data.OwnerPlayerIndex].Assists, data.LocationIndex);
                    newDesignName = "NoResource";
                }

                cc.SetCardDesign(newDesignName);
            }

            foreach (var cardData in newCards)
            {
                if (m_cardControls.Any(cc => cc.CardGuid == cardData.Guid))
                {
                    continue;
                }

                var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("Normal"), cardData.Guid);
                ccStyle.Initialize();

                var cardControl = ccStyle.TypedTarget;
                cardControl.CardData = cardData;
                cardControl.MouseTracked.MouseButton1Up += CardControl_MouseButton1Up;
                cardControl.MouseTracked.MouseButton2Down += CardControl_MouseButton2Down;
                cardControl.Addins.Add(new UI.CardControlAddins.Highlight(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.DamageIndicator(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.InstantRotation(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.Flip(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.LocationAnimation(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.ToneAnimation(cardControl));
                cardControl.Addins.Add(new UI.CardControlAddins.CardIcons(cardControl));
                m_cardControls.Add(cardControl);

                if (cardData.Location == CardDataManager.CardLocation.Hand)
                {
                    PutToLibrary(cardControl);
                }
            }

            m_cardData = newCards;

            foreach (var pz in m_playerZones)
            {
                pz.Hand.Rearrange();
                pz.Battlefield.Rearrange();
                pz.Sacrifice.Rearrange();
            }
            m_actingLocalPlayerHandZone.Rearrange();

            m_cardControls.ForEach(cc => cc.Update(deltaTime));
        }

        private void UnregisterAllCards()
        {
            m_cardControls.ForEach(cc => cc.Dispose());
            m_cardControls.Clear();
        }

        private void CardControl_MouseButton1Up(object sender, UI.MouseEventArgs e)
        {
            var control = (sender as UI.MouseTracked).EventTarget as UI.CardControl;
            Debug.Assert(control != null);
            if (IsCardClickable(control))
            {
                UIState.OnCardClicked(control);
            }
        }

        private void CardControl_MouseButton2Down(object sender, UI.MouseEventArgs e)
        {
            var control = (sender as UI.MouseTracked).EventTarget as UI.CardControl;
            ZoomInCard(control);
        }
    }
}
