using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private class PlayerZones : Style.IBindingProvider
        {
            private GameEvaluator<string> m_manaTextEvaluator;

            public Style.IStyleContainer UIStyle
            {
                get; private set;
            }

            public UI.EventDispatcher UIRoot
            {
                get { return UIStyle.Target; }
            }

            public int PlayerIndex
            {
                get; private set;
            }

            public CardZone Library
            {
                get; private set;
            }

            public CardZone Hand
            {
                get; private set;
            }

            public CardZone Sacrifice
            {
                get; private set;
            }

            public CardZone Battlefield
            {
                get; private set;
            }

            public CardZone Hero
            {
                get; private set;
            }

            public CardZone Assists
            {
                get; private set;
            }

            public CardZone Graveyard
            {
                get; private set;
            }

            public PlayerZones(int playerIndex, Style.IStyleContainer parent, XElement styleDefinition)
            {
                var layout = new Style.LayoutGizmo<UI.TransformNode>(parent, styleDefinition);
                layout.Initialize();
                layout.Target.Dispatcher = parent.Target;
                layout.BindingProvider = this;
                UIStyle = layout;
                PlayerIndex = playerIndex;

                m_manaTextEvaluator = GameApp.Service<GameManager>().CreateGameEvaluator(game =>
                {
                    var player = game.Players[PlayerIndex];
                    return player.Mana != 0 || player.MaxMana != 0
                           ? player.Mana.ToString() + "/" + player.MaxMana.ToString()
                           : "0";
                }, "-");

                Library = new CardZone(UIStyle.ChildIds["Library"]);
                Hand = new CardZone(UIStyle.ChildIds["Hand"]);
                Sacrifice = new CardZone(UIStyle.ChildIds["Sacrifice"]);
                Battlefield = new CardZone(UIStyle.ChildIds["Battlefield"]);
                Hero = new CardZone(UIStyle.ChildIds["Hero"]);
                Assists = new CardZone(UIStyle.ChildIds["Assists"]);
                Graveyard = new CardZone(UIStyle.ChildIds["Graveyard"]);
            }

            public bool EvaluateBinding(string id, out string replacement)
            {
                switch (id)
                {
                    case "Player.ManaPoolText":
                        replacement = m_manaTextEvaluator.Value;
                        break;

                    default:
                        replacement = null;
                        return false;
                }

                return true;
            }
        }
    }
}
