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

        public void StartGame(params GameStartupParameters[] parameters)
        {
            Game = new Game(parameters.ToIndexable(), new XnaUIController());
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
