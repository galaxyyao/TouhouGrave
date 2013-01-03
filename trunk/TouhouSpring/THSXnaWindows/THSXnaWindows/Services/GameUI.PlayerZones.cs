using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private class PlayerZones
        {
            public Style.IStyleContainer UIStyle
            {
                get; private set;
            }

            public UI.EventDispatcher UIRoot
            {
                get { return UIStyle.Target; }
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

            public PlayerZones(Style.IStyleContainer parent, XElement styleDefinition)
            {
                UIStyle = new Style.LayoutGizmo(parent, styleDefinition);
                UIStyle.Initialize();
                UIStyle.Target.Dispatcher = parent.Target;

                Library = new CardZone(UIStyle.ChildIds["Library"]);
                Hand = new CardZone(UIStyle.ChildIds["Hand"]);
                Sacrifice = new CardZone(UIStyle.ChildIds["Sacrifice"]);
                Battlefield = new CardZone(UIStyle.ChildIds["Battlefield"]);
                Hero = new CardZone(UIStyle.ChildIds["Hero"]);
                Assists = new CardZone(UIStyle.ChildIds["Assists"]);
                Graveyard = new CardZone(UIStyle.ChildIds["Graveyard"]);
            }
        }
    }
}
