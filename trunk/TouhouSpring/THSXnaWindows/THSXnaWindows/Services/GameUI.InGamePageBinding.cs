using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameUI : Style.IBindingProvider
    {
        private class CachedGameValues
        {
            public string m_currentPhase = "";
            public int m_actingPlayerIndex = -1;
            public string m_player0Name = "-";
            public string m_player0Health = "-";
            public string m_player1Name = "-";
            public string m_player1Health = "-";
        }

        private CachedGameValues m_cachedGameValues = new CachedGameValues();
        private Services.GameEvaluator m_evaluator;

        bool Style.IBindingProvider.EvaluateBinding(string id, out string replacement)
        {
            switch (id)
            {
                case "Game.AlignToScreenTransform":
                    replacement = WorldCamera.AlignToNearPlaneMatrix.Serialize();
                    break;
                case "Game.DetailText1":
                    replacement = ZoomedInCard != null
                                  ? ZoomedInCard.Card.Model.Name : "";
                    break;
                case "Game.DetailText2":
                    if (ZoomedInCard != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("【#Card.SystemClass#】\n");
                        if (ZoomedInCard.Card.Behaviors.Has<Behaviors.ManaCost>())
                        {
                            sb.Append("召唤消耗　【[color:Red]#Card.SummonCost#[/color]】 灵力\n");
                        }
                        if (ZoomedInCard.Card.Behaviors.Has<Behaviors.Warrior>())
                        {
                            sb.Append("攻击力　　【[color:Red]#Card.InitialAttack#[/color]】\n");
                            sb.Append("体力　　　【[color:Red]#Card.InitialLife#[/color]】\n");
                        }
                        sb.Append("\n");
                        sb.Append(ZoomedInCard.Card.Model.Description);
                        replacement = sb.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                case "Game.Phase":
                    replacement = m_cachedGameValues.m_currentPhase;
                    break;
                case "Game.Player0.Avatar":
                    replacement = "Textures/natsukawa";
                    break;
                case "Game.Player0.AvatarBorder":
                    replacement = m_cachedGameValues.m_actingPlayerIndex == 0
                                  ? "atlas:Textures/UI/InGame/Atlas0$AvatarBorderActive"
                                  : "atlas:Textures/UI/InGame/Atlas0$AvatarBorderInactive";
                    break;
                case "Game.Player0.Name":
                    replacement = m_cachedGameValues.m_player0Name;
                    break;
                case "Game.Player0.Health":
                    replacement = m_cachedGameValues.m_player0Health;
                    break;
                case "Game.Player1.Avatar":
                    replacement = "Textures/fuyumi";
                    break;
                case "Game.Player1.AvatarBorder":
                    replacement = m_cachedGameValues.m_actingPlayerIndex == 1
                                  ? "atlas:Textures/UI/InGame/Atlas0$AvatarBorderActive"
                                  : "atlas:Textures/UI/InGame/Atlas0$AvatarBorderInactive";
                    break;
                case "Game.Player1.Name":
                    replacement = m_cachedGameValues.m_player1Name;
                    break;
                case "Game.Player1.Health":
                    replacement = m_cachedGameValues.m_player1Health;
                    break;
                case "Game.ResolutionWidth":
                    replacement = GameApp.Instance.GraphicsDevice.Viewport.Width.ToString();
                    break;
                case "Game.ResolutionHeight":
                    replacement = GameApp.Instance.GraphicsDevice.Viewport.Height.ToString();
                    break;
                case "Game.UICamera.Transform":
                    replacement = UICamera.WorldToProjectionMatrix.Serialize();
                    break;
                case "Game.WorldCamera.Transform":
                    replacement = WorldCamera.WorldToProjectionMatrix.Serialize();
                    break;
                case "Conversation.CurrentText":
                    replacement = GameApp.Service<Services.ConversationUI>().CurrentText;
                    break;
                default:
                    if (id.StartsWith("Card.") && ZoomedInCard != null)
                    {
                        return ZoomedInCard.EvaluateBinding(id, out replacement);
                    }
                    replacement = null;
                    return false;
            }

            return true;
        }

        private void CreateBindingEvaluator()
        {
            m_evaluator = GameApp.Service<Services.GameManager>().CreateGameEvaluator(game =>
            {
                m_cachedGameValues.m_currentPhase = game.CurrentPhase;
                m_cachedGameValues.m_actingPlayerIndex = game.ActingPlayer != null ? game.ActingPlayer.Index : -1;
                m_cachedGameValues.m_player0Name = game.Players.Count > 0 ? game.Players[0].Name : "-";
                m_cachedGameValues.m_player0Health = game.Players.Count > 0 ? game.Players[0].Health.ToString() : "-";
                m_cachedGameValues.m_player1Name = game.Players.Count > 1 ? game.Players[1].Name : "-";
                m_cachedGameValues.m_player1Health = game.Players.Count > 1 ? game.Players[1].Health.ToString() : "-";
            });
        }
    }
}
