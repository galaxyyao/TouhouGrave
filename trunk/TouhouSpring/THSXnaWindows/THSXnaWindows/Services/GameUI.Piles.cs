using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private struct GraveyardCounters
        {
            public int nextCounter;
            public int currentCounter;
        }

        private UI.CardControl[] m_playerLibraryPiles;
        private UI.CardControl[] m_playerGraveyardPiles;
        private List<UI.CardControl> m_cardEnteringGraveyard = new List<UI.CardControl>();
        private GraveyardCounters[] m_graveyardCounters;

        private GameEvaluator<int>[] m_playerLibraryPileCountEvaluators;

        private void InitializePilesOnGameCreated(Game game)
        {
            var numPlayers = game.Players.Count;
            m_playerLibraryPiles = new UI.CardControl[numPlayers];
            m_playerLibraryPileCountEvaluators = new GameEvaluator<int>[numPlayers];
            for (int i = 0; i < numPlayers; ++i)
            {
                var dummyCard = CardInstance.CreateDummyCard(game.Players[i]);
                var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("PileBack"), dummyCard);
                ccStyle.Initialize();
                m_playerLibraryPiles[i] = ccStyle.TypedTarget;
                m_playerLibraryPiles[i].EnableDepth = true;

                int pid = i; // to force creating new lambdas based on the current value of i
                m_playerLibraryPileCountEvaluators[i] = GameApp.Service<GameManager>().CreateGameEvaluator(g => g.Players[pid].Library.Count, 0);
                m_playerLibraryPiles[i].Addins.Add(new UI.CardControlAddins.InstantRotation(m_playerLibraryPiles[i]));
                m_playerLibraryPiles[i].Addins.Add(new UI.CardControlAddins.Pile(m_playerLibraryPiles[i],
                    () => m_playerLibraryPileCountEvaluators[pid].Value));
                m_playerLibraryPiles[i].Dispatcher = m_playerZones[i].Library.Container;
            }

            // graveyard piles are created only when there are more than 1 cards in the graveyard
            // the last card entering graveyard will be displayed on the top
            m_playerGraveyardPiles = new UI.CardControl[numPlayers];
            m_graveyardCounters = new GraveyardCounters[numPlayers];
        }

        private void PutToLibrary(UI.CardControl cardControl)
        {
            cardControl.Style.Apply(); // to solve the default TransformToGlobal matrix
            var transform = cardControl.BodyContainer.TransformToGlobal.Invert();

            cardControl.GetAddin<UI.CardControlAddins.Flip>().SetFliped();
            cardControl.Style.Apply(); // to apply initial flipped matrix

            var fromZone = m_playerZones[cardControl.CardData.OwnerPlayerIndex].Library.Container;
            var pileTop = m_playerLibraryPiles[cardControl.CardData.OwnerPlayerIndex].BodyContainer;
            transform *= UI.TransformNode.GetTransformBetween(pileTop, fromZone);

            cardControl.Dispatcher = fromZone;
            cardControl.Transform = transform;
        }

        private void PutToGraveyard(UI.CardControl cardControl)
        {
            var locationAnim = cardControl.GetAddin<UI.CardControlAddins.LocationAnimation>();
            locationAnim.SetNextLocation(
                m_playerZones[cardControl.CardData.OwnerPlayerIndex].Graveyard,
                m_graveyardCounters[cardControl.CardData.OwnerPlayerIndex].nextCounter++);
            locationAnim.Update(0); // make sure InTransition returns true
            m_cardEnteringGraveyard.Add(cardControl);
        }

        private void EnteredGraveyard(UI.CardControl cardControl)
        {
            var pid = cardControl.CardData.OwnerPlayerIndex;
            if (m_playerGraveyardPiles[pid] != null)
            {
                m_playerGraveyardPiles[pid].Dispose();
            }
            cardControl.Addins.Clear();
            cardControl.Addins.Add(new UI.CardControlAddins.InstantRotation(cardControl));
            cardControl.MouseTracked.MouseButton1Up -= CardControl_MouseButton1Up;
            cardControl.MouseTracked.MouseButton2Down -= CardControl_MouseButton2Down;
            cardControl.Saturate = 0;

            var graveyardPileHeight = ++m_graveyardCounters[pid].currentCounter;
            if (graveyardPileHeight > 1)
            {
                cardControl.Addins.Add(new UI.CardControlAddins.Pile(cardControl, () => graveyardPileHeight - 1));
            }

            m_playerGraveyardPiles[pid] = cardControl;
        }

        private void UpdatePiles(float deltaTime)
        {
            for (int i = 0; i < m_cardEnteringGraveyard.Count; ++i)
            {
                var cc = m_cardEnteringGraveyard[i];
                if (!cc.GetAddin<UI.CardControlAddins.LocationAnimation>().InTransition)
                {
                    EnteredGraveyard(cc);
                    m_cardEnteringGraveyard.RemoveAt(i);
                    --i;
                    continue;
                }
                cc.Update(deltaTime);
            }

            foreach (var pile in m_playerLibraryPiles)
            {
                pile.Update(deltaTime);
            }
            foreach (var pile in m_playerGraveyardPiles)
            {
                if (pile != null)
                {
                    pile.Update(deltaTime);
                }
            }
        }
    }
}
