using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameManager : GameService
    {
        private Game m_game;
        private GameEvaluator<int> m_actingPlayerIndexEvaluator;

        public int ActingPlayerIndex
        {
            get { return m_actingPlayerIndexEvaluator.Value; }
        }

        public IIndexable<Agents.BaseAgent> Agents
        {
            get;
            private set;
        }

        public override void Startup()
        {
            m_actingPlayerIndexEvaluator = CreateGameEvaluator(game => game.ActingPlayer != null ? game.ActingPlayer.Index : -1, -1);
        }

        public void StartGame(GameStartupParameters parameters, Agents.BaseAgent[] agents)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            else if (agents == null)
            {
                throw new ArgumentNullException("agents");
            }
            else if (parameters.PlayerDecks.Count != agents.Length)
            {
                throw new InvalidOperationException("PlayerDecks and agents shall have the same length.");
            }

            Agents = agents.ToIndexable();

            m_game = new Game(parameters.PlayerDecks, parameters.PlayerIds, new XnaUIController(agents), parameters.Seed);

            GameApp.Service<GameUI>().GameCreated(m_game);
            GameApp.Service<Graphics.Scene>().GameCreated();

            m_game.StartGameFlowThread();
            GameApp.Service<Sound>().PlayMusic(Sound.MusicEnum.kagamiM);
        }

        public override void Update(float deltaTime)
        {
            if (m_game != null)
            {
                m_game.Controller.ProcessMessage();
            }
        }

        public void EnterConversation()
        {
            GameApp.Service<ConversationUI>().StartConversation();
        }

        public void EnterNetworkUI()
        {
            GameApp.Service<NetworkLoginUI>().Enter();
        }
    }
}
