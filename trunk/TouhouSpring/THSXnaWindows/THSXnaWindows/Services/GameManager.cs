using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    class GameManager : GameService
    {
        public Game Game
        {
            get; private set;
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

            Game = new Game(parameters.ToIndexable(), new XnaUIController(agents));
            GameApp.Service<GameUI>().GameStarted();
            GameApp.Service<Graphics.Scene>().GameStarted();
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
    }
}
