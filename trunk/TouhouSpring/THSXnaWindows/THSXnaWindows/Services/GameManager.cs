using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameManager : GameService
    {
        public Game Game
        {
            get; private set;
        }

        private GameEvaluator<int> m_actingPlayerIndexEvaluator;

        public int ActingPlayerIndex
        {
            get { return m_actingPlayerIndexEvaluator.Value; }
        }

        public IIndexable<Agents.BaseAgent> Agents
        {
            get; private set;
        }

        public override void Startup()
        {
            m_actingPlayerIndexEvaluator = CreateGameEvaluator(game => game.ActingPlayer != null ? game.ActingPlayer.Index : -1, -1);
        }

        public void StartGame(GameStartupParameters[] parameters, Agents.BaseAgent[] agents)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            else if (agents == null)
            {
                throw new ArgumentNullException("agents");
            }
            else if (parameters.Length != agents.Length)
            {
                throw new InvalidOperationException("Parameters and agents shall have the same length.");
            }

            Agents = agents.ToIndexable();

            Game = new Game(parameters.ToIndexable(), new XnaUIController(agents));

            GameApp.Service<GameUI>().GameCreated(Game);
            GameApp.Service<Graphics.Scene>().GameCreated();

            Game.StartGameFlowThread();
        }

        public override void Update(float deltaTime)
        {
            if (Game == null)
            {
                return;
            }

            Game.Controller.ProcessMessage();
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
