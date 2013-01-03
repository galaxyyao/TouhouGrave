﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Properties;
using TouhouSpring.UI;

namespace TouhouSpring.Style
{
    class PageStyle : BaseStyleContainer, IBindingProvider
    {
        public Page TypedTarget
        {
            get { return (Page)Target; }
        }

        public override IEnumerable<IBindingProvider> BindingProviders
        {
            get { yield return this; }
        }

        public override Rectangle Bounds
        {
            get { return new Rectangle(0, 0, TypedTarget.Width, TypedTarget.Height); }
            protected set { throw new NotSupportedException(); }
        }

        public PageStyle(XElement definition)
            : base(null, definition)
        { }

        public override void Initialize()
        {
            PreInitialize(() => new Page(this));

            if (Definition == null)
            {
                return;
            }

            foreach (var childElement in Definition.Elements())
            {
                if (childElement.Name == "Image")
                {
                    AddChildAndInitialize(new ImageStyle(this, childElement));
                }
                else if (childElement.Name == "Label")
                {
                    AddChildAndInitialize(new LabelStyle(this, childElement));
                }
                else if (childElement.Name == "Layout")
                {
                    AddChildAndInitialize(new LayoutGizmo(this, childElement));
                }
                else if (childElement.Name == "Menu")
                {
                    AddChildAndInitialize(new MenuStyle(this, childElement));
                }
                else if (childElement.Name == "Panel")
                {
                    AddChildAndInitialize(new PanelStyle(this, childElement));
                }
            }
        }

        public bool TryGetValue(string id, out string replacement)
        {
            var game = GameApp.Service<Services.GameManager>().Game;
            var gameui = GameApp.Service<Services.GameUI>();
            switch (id)
            {
                case "Game.AlignToScreenTransform":
                    replacement = gameui.WorldCamera.AlignToNearPlaneMatrix.Serialize();
                    break;
                case "Game.DetailText1":
                    replacement = gameui.ZoomedInCard != null
                                  ? gameui.ZoomedInCard.Card.Model.Name : "";
                    break;
                case "Game.DetailText2":
                    if (gameui.ZoomedInCard != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("【#Card.SystemClass#】\n");
                        if (gameui.ZoomedInCard.Card.Behaviors.Has<Behaviors.ManaCost>())
                        {
                            sb.Append("召唤消耗　【[color:Red]#Card.SummonCost#[/color]】 灵力\n");
                        }
                        if (gameui.ZoomedInCard.Card.Behaviors.Has<Behaviors.Warrior>())
                        {
                            sb.Append("攻击力　　【[color:Red]#Card.InitialAttack#[/color]】\n");
                            sb.Append("体力　　　【[color:Red]#Card.InitialLife#[/color]】\n");
                        }
                        sb.Append("\n");
                        sb.Append(gameui.ZoomedInCard.Card.Model.Description);
                        replacement = sb.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                case "Game.Phase":
                    replacement = game.CurrentPhase;
                    break;
                case "Game.Player0.Avatar":
                    replacement = "Textures/Yozora";
                    break;
                case "Game.Player0.AvatarBorder":
                    replacement = game.ActingPlayer == game.Players[0]
                                  ? "Textures/AvatarBorderActive" : "Textures/AvatarBorderInactive";
                    break;
                case "Game.Player0.Name":
                    replacement = game.Players[0].Name;
                    break;
                case "Game.Player0.Health":
                    replacement = game.Players[0].Health.ToString();
                    break;
                case "Game.Player1.Avatar":
                    replacement = "Textures/Sena";
                    break;
                case "Game.Player1.AvatarBorder":
                    replacement = game.ActingPlayer == game.Players[1]
                                  ? "Textures/AvatarBorderActive" : "Textures/AvatarBorderInactive";
                    break;
                case "Game.Player1.Name":
                    replacement = game.Players[1].Name;
                    break;
                case "Game.Player1.Health":
                    replacement = game.Players[1].Health.ToString();
                    break;
                case "Game.ResolutionWidth":
                    replacement = GameApp.Instance.GraphicsDevice.Viewport.Width.ToString();
                    break;
                case "Game.ResolutionHeight":
                    replacement = GameApp.Instance.GraphicsDevice.Viewport.Height.ToString();
                    break;
                case "Game.UICamera.Transform":
                    replacement = gameui.UICamera.WorldToProjectionMatrix.Serialize();
                    break;
                case "Game.WorldCamera.Transform":
                    replacement = gameui.WorldCamera.WorldToProjectionMatrix.Serialize();
                    break;
                case "Conversation.CurrentText":
                    replacement = GameApp.Service<Services.ConversationUI>().CurrentText;
                    break;
                default:
                    if (id.StartsWith("Card.") && gameui.ZoomedInCard != null)
                    {
                        return gameui.ZoomedInCard.TryGetValue(id, out replacement);
                    }
                    replacement = null;
                    return false;
            }

            return true;
        }
    }
}
