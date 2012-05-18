﻿using System;
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
			Game = new Game(parameters.ToIndexable());
			GameApp.Service<GameUI>().GameStarted();
			GameApp.Service<Graphics.Scene>().GameStarted();
		}

		public override void Update(float deltaTime)
		{
			if (Game == null)
			{
				return;
			}

			foreach (var controller in Game.Controllers)
			{
				controller.ProcessMessage();
			}
		}

        public void EnterConversation()
        {
            GameApp.Service<ConversationUI>().StartConversation();
        }

	}
}
