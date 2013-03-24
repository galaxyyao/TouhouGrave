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
        private GameEvaluator m_cardControlEvaluator;

        public void RegisterCard(CardInstance card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }
            else if (m_cardControls.Any(cc => cc.Card == card))
            {
                throw new ArgumentException("The card is already registered.", "card");
            }

            var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("Normal"), card);
            ccStyle.Initialize();

            var cardControl = ccStyle.TypedTarget;
            cardControl.OnEvaluate(card);
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

            if (card.Owner.CardsOnHand.Contains(card))
            {
                PutToLibrary(cardControl);
            }
        }

        public void UnregisterCard(CardInstance card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            var index = m_cardControls.FindIndex(cc => cc.Card == card);
            var cardControl = m_cardControls[index];
            m_cardControls.RemoveAt(index);
            PutToGraveyard(cardControl);
        }

        public bool TryGetCardControl(CardInstance card, out UI.CardControl cardControl)
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

        private void InitializeCardControls()
        {
            m_cardControlEvaluator = GameApp.Service<GameManager>().CreateGameEvaluator(game => CardControlOnGameEvaluate(game));
        }

        private void CardControlOnGameEvaluate(Game game)
        {
            foreach (var cc in m_cardControls)
            {
                cc.OnEvaluate(cc.Card);
            }
        }

        private void UnregisterAllCards()
        {
            m_cardControls.ForEach(cc => cc.Dispose());
            m_cardControls.Clear();
        }

        private void UpdateCardControls(float deltaTime)
        {
            UpdateCardLocations();
            m_cardControls.ForEach(cc => cc.Update(deltaTime));
        }

        private void CardControl_MouseButton1Up(object sender, UI.MouseEventArgs e)
        {
            var control = (sender as UI.MouseTracked).EventTarget as UI.CardControl;
            Debug.Assert(control != null && control.Card != null);
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
