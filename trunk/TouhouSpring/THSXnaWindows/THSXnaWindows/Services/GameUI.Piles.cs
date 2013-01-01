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
        private UI.CardControl[] m_playerLibraryPiles;

        private void RegisterPiles()
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
                m_playerLibraryPiles[i].Addins.Add(new UI.CardControlAddins.Pile(m_playerLibraryPiles[i], Game.Players[i].Library));
                m_playerLibraryPiles[i].Dispatcher = m_playerZones[i].m_library.Container;
            }
        }

        private void InitializeToLibrary(UI.CardControl cardControl)
        {
            cardControl.Style.Apply(); // to solve the default TransformToGlobal matrix
            var fromPile = m_playerLibraryPiles[Game.Players.IndexOf(cardControl.Card.Owner)];
            var pileTop = fromPile.Style.ChildIds["Body"].Target;
            var transform = (cardControl.Style.ChildIds["Body"].Target as UI.ITransformNode).TransformToGlobal.Invert();

            cardControl.GetAddin<UI.CardControlAddins.Flip>().DoFlip = false;
            cardControl.GetAddin<UI.CardControlAddins.Flip>().StartFlip();
            cardControl.Style.Apply(); // to apply initial flipped matrix

            cardControl.Dispatcher = pileTop;
            cardControl.Transform = transform;
        }

        private void UpdatePiles(float deltaTime)
        {
            foreach (var pile in m_playerLibraryPiles)
            {
                pile.Update(deltaTime);
            }
        }
    }
}
