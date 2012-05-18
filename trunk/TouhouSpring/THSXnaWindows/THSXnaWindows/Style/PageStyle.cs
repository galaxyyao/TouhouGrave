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
			switch (id)
			{
				case "Game.AlignToScreenTransform":
					replacement = GameApp.Service<Services.GameUI>().WorldCamera.AlignToNearPlaneMatrix.Serialize();
					break;
                case "Game.DetailText1":
                    replacement = GameApp.Service<Services.GameUI>().ZoomedInCard != null
                                  ? GameApp.Service<Services.GameUI>().ZoomedInCard.Card.Model.Name : "";
                    break;
                case "Game.DetailText2":
                    replacement = GameApp.Service<Services.GameUI>().ZoomedInCard != null
                                  ? GameApp.Service<Services.GameUI>().ZoomedInCard.Card.Model.Description : "";
                    break;
				case "Game.Phase":
					replacement = GameApp.Service<Services.GameManager>().Game.CurrentPhase;
					break;
				case "Game.Player0.Avatar":
					replacement = "Textures/Yozora";
					break;
				case "Game.Player0.AvatarBorder":
					replacement = game.InPlayerPhases && game.PlayerPlayer == game.Players[0]
								  ? "Textures/AvatarBorderActive" : "Textures/AvatarBorderInactive";
					break;
				case "Game.Player0.Name":
					replacement = game.InPlayerPhases ? game.Players[0].Name : "...";
					break;
				case "Game.Player0.Health":
					replacement = game.InPlayerPhases ? game.Players[0].Health.ToString() : "...";
					break;
				case "Game.Player0.Mana":
                    replacement = game.InPlayerPhases ? game.Players[0].Mana.ToString() : "...";
                    break;
				case "Game.Player1.Avatar":
					replacement = "Textures/Sena";
					break;
				case "Game.Player1.AvatarBorder":
					replacement = game.InPlayerPhases && game.PlayerPlayer == game.Players[1]
								  ? "Textures/AvatarBorderActive" : "Textures/AvatarBorderInactive";
					break;
				case "Game.Player1.Name":
					replacement = game.InPlayerPhases ? game.Players[1].Name : "...";
					break;
				case "Game.Player1.Health":
					replacement = game.InPlayerPhases ? game.Players[1].Health.ToString() : "...";
					break;
				case "Game.Player1.Mana":
                    replacement = game.InPlayerPhases ? game.Players[1].Mana.ToString() : "...";
                    break;
				case "Game.ResolutionWidth":
					replacement = GameApp.Instance.GraphicsDevice.Viewport.Width.ToString();
					break;
				case "Game.ResolutionHeight":
					replacement = GameApp.Instance.GraphicsDevice.Viewport.Height.ToString();
					break;
				case "Game.UICamera.Transform":
					replacement = GameApp.Service<Services.GameUI>().UICamera.WorldToProjectionMatrix.Serialize();
					break;
				case "Game.WorldCamera.Transform":
					replacement = GameApp.Service<Services.GameUI>().WorldCamera.WorldToProjectionMatrix.Serialize();
					break;
                case "Conversation.CurrentText":
                    replacement = GameApp.Service<Services.ConversationUI>().CurrentText;
                    break;
				default:
					replacement = null;
					return false;
			}

			return true;
		}
	}
}
