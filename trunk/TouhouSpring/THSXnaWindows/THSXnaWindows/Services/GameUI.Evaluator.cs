using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        private class CachedGameValues
        {
            public string m_currentPhase = "";
            public string m_player0Name = "-";
            public string m_player0Health = "-";
            public string m_player1Name = "-";
            public string m_player1Health = "-";
        }

        private CachedGameValues m_cachedGameValues = new CachedGameValues();
        private Services.GameEvaluator m_evaluator;

        private void CreateBindingEvaluator()
        {
            m_evaluator = GameApp.Service<Services.GameManager>().CreateGameEvaluator(game => OnGameEvaluate(game));
        }

        private void OnGameEvaluate(Game game)
        {
            m_cachedGameValues.m_currentPhase = game.CurrentPhase;
            m_cachedGameValues.m_player0Name = game.Players.Count > 0 ? game.Players[0].Name : "-";
            m_cachedGameValues.m_player0Health = game.Players.Count > 0 ? game.Players[0].Health.ToString() : "-";
            m_cachedGameValues.m_player1Name = game.Players.Count > 1 ? game.Players[1].Name : "-";
            m_cachedGameValues.m_player1Health = game.Players.Count > 1 ? game.Players[1].Health.ToString() : "-";
        }
    }
}
