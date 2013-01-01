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
        private List<UI.CardControl> m_cardControls = new List<UI.CardControl>();

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
            cardControl.MouseTracked.MouseButton1Up += (s, e) => OnCardClicked(cardControl);
            cardControl.MouseTracked.MouseButton2Down += (s, e) => ZoomInCard(cardControl);
            cardControl.Addins.Add(new UI.CardControlAddins.Highlight(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.DamageIndicator(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.Flip(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.LocationAnimation(cardControl));
            cardControl.Addins.Add(new UI.CardControlAddins.ToneAnimation(cardControl));
            m_cardControls.Add(cardControl);

            if (card.Owner.CardsOnHand.Contains(card))
            {
                InitializeToLibrary(cardControl);
            }
        }

        public void UnregisterCard(BaseCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            if (card.Owner.Hero == card)
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
            if (IsCardClickable(control))
            {
                UIState.OnCardClicked(control);
            }
        }
    }
}
