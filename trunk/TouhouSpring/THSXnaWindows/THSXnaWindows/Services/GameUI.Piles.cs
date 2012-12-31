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
        private UI.CardControl m_playerLibraryPile;
        private UI.CardControl m_opponentLibraryPile;

        private void RegisterPiles()
        {
            var dummyModel = new CardModel
            {
                Behaviors = new List<Behaviors.BehaviorModel>(),
                Name = ""
            };
            var dummyCard = new BaseCard(dummyModel, null);

            {
                var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("PileBack"), dummyCard);
                ccStyle.Initialize();
                m_playerLibraryPile = ccStyle.TypedTarget;
                m_playerLibraryPile.Addins.Add(new UI.CardControlAddins.Pile(m_playerLibraryPile, Game.Players[0].Library));
                m_playerLibraryPile.Dispatcher = m_playerLibraryZoneInfo.m_container;
            }

            {
                var ccStyle = new Style.CardControlStyle(GameApp.Service<Styler>().GetCardStyle("PileBack"), dummyCard);
                ccStyle.Initialize();
                m_opponentLibraryPile = ccStyle.TypedTarget;
                m_opponentLibraryPile.Addins.Add(new UI.CardControlAddins.Pile(m_playerLibraryPile, Game.Players[0].Library));
                //m_opponentLibraryPile.Dispatcher = m_opponentLibraryZoneInfo.m_container;
            }
        }

        private void UpdatePiles(float deltaTime)
        {
            if (m_playerLibraryPile != null)
            {
                m_playerLibraryPile.Update(deltaTime);
                m_opponentLibraryPile.Update(deltaTime);
            }
        }
    }
}
