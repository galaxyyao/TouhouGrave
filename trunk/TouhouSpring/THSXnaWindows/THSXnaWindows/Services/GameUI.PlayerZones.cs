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
            public Style.IStyleContainer UIStyle
            {
                get; private set;
            }

            public UI.EventDispatcher UIRoot
            {
                get { return UIStyle.Target; }
            }

            public Player Player
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

            public PlayerZones(Player player, Style.IStyleContainer parent, XElement styleDefinition)
            {
                var layout = new Style.LayoutGizmo(parent, styleDefinition);
                layout.Initialize();
                layout.Target.Dispatcher = parent.Target;
                layout.BindingProvider = this;
                UIStyle = layout;
                Player = player;

                Library = new CardZone(UIStyle.ChildIds["Library"]);
                Hand = new CardZone(UIStyle.ChildIds["Hand"]);
                Sacrifice = new CardZone(UIStyle.ChildIds["Sacrifice"]);
                Battlefield = new CardZone(UIStyle.ChildIds["Battlefield"]);
                Hero = new CardZone(UIStyle.ChildIds["Hero"]);
                Assists = new CardZone(UIStyle.ChildIds["Assists"]);
                Graveyard = new CardZone(UIStyle.ChildIds["Graveyard"]);
            }

            public bool TryGetValue(string id, out string replacement)
            {
                switch (id)
                {
                    case "Player.ManaPoolText":
                        if (Player.Mana == 0 && Player.MaxMana == 0)
                        {
                            replacement = "0";
                        }
                        else
                        {
                            replacement = Player.Mana.ToString() + "/" + Player.MaxMana.ToString();
                        }
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
