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

        private void InitializePiles()
        {
            var dummyModel = new CardModel
            {
                Behaviors = new List<Behaviors.BehaviorModel>(),
                Name = ""
            };
            var dummyCard = new BaseCard(dummyModel, null);

            m_playerLibraryPiles = new UI.CardControl[Game.Players.Count];
            for (int i = 0; i < Game.Players.Count; ++i)
            {
                var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("PileBack"), dummyCard);
                ccStyle.Initialize();
                m_playerLibraryPiles[i] = ccStyle.TypedTarget;
                m_playerLibraryPiles[i].EnableDepth = true;
                var pile = Game.Players[i].Library;
                m_playerLibraryPiles[i].Addins.Add(new UI.CardControlAddins.Pile(m_playerLibraryPiles[i], () => pile.Count));
                m_playerLibraryPiles[i].Dispatcher = m_playerZones[i].Library.Container;
            }

            // graveyard piles are created only when there are more than 1 cards in the graveyard
            // the last card entering graveyard will be displayed on the top
            m_playerGraveyardPiles = new UI.CardControl[Game.Players.Count];
            m_graveyardCounters = new GraveyardCounters[Game.Players.Count];
        }

        private void PutToLibrary(UI.CardControl cardControl)
        {
            cardControl.Style.Apply(); // to solve the default TransformToGlobal matrix
            var transform = (cardControl.Style.ChildIds["Body"].Target as UI.ITransformNode).TransformToGlobal.Invert();

            cardControl.GetAddin<UI.CardControlAddins.Flip>().SetFliped();
            cardControl.Style.Apply(); // to apply initial flipped matrix

            var pid = Game.Players.IndexOf(cardControl.Card.Owner);
            var fromZone = m_playerZones[pid].Library.Container;
            var pileTop = m_playerLibraryPiles[pid].Style.ChildIds["Body"].Target;
            transform *= UI.TransformNode.GetTransformBetween(pileTop, fromZone);

            cardControl.Dispatcher = fromZone;
            cardControl.Transform = transform;
        }

        private void PutToGraveyard(UI.CardControl cardControl)
        {
            var pid = Game.Players.IndexOf(cardControl.Card.Owner);
            var locationAnim = cardControl.GetAddin<UI.CardControlAddins.LocationAnimation>();
            locationAnim.SetNextLocation(m_playerZones[pid].Graveyard, m_graveyardCounters[pid].nextCounter++);
            locationAnim.Update(0); // make sure InTransition returns true
            m_cardEnteringGraveyard.Add(cardControl);
        }

        private void EnteredGraveyard(UI.CardControl cardControl)
        {
            var pid = Game.Players.IndexOf(cardControl.Card.Owner);
            if (m_playerGraveyardPiles[pid] != null)
            {
                m_playerGraveyardPiles[pid].Dispose();
            }
            cardControl.Addins.Clear();
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
