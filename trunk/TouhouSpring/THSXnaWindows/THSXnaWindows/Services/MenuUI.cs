using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(CardDatabase))]
	[LifetimeDependency(typeof(ResourceManager))]
	[LifetimeDependency(typeof(Styler))]
	[LifetimeDependency(typeof(UIManager))]
	[UpdateDependency(typeof(UIManager))]
	partial class MenuUI : GameService
	{
		private Dictionary<string, MenuPage> m_pages = new Dictionary<string, MenuPage>();

		public UI.TransformNode Root
		{
			get; private set;
		}

		public override void Startup()
		{
			Matrix toScreenSpace = Matrix.Identity;
			toScreenSpace.M11 = 2 / 1024.0f;
			toScreenSpace.M22 = 2 / 768.0f;
			toScreenSpace.M41 = -1;
			toScreenSpace.M42 = -1;

			var cam = new Camera
			{
				PostWorldMatrix = toScreenSpace,
				Position = Vector3.UnitZ,
				IsPerspective = false,
				ViewportWidth = 2,
				ViewportHeight = -2
			};
			cam.Dirty();

			Root = new UI.TransformNode
			{
				Transform = cam.WorldToProjectionMatrix,
				Dispatcher = GameApp.Service<UIManager>().Root
			};

			LoadPage("MainMenu");
			LoadPage("FreeMode");
			LoadPage("Quit");

			m_pages["MainMenu"].MenuClicked += (id, item) =>
			{
				if (id == "freemode")
				{
					CurrentPage = m_pages["FreeMode"];
				}
                //else if (id == "storymode")
                //{
                //    //Test Conversation UI
                //    CurrentPage = null;
                //    Root.Dispatcher = null;
                //    GameApp.Service<GameManager>().EnterConversation();
                //}
                else if (id == "quit")
                {
                    CurrentPage = m_pages["Quit"];
                }
			};

			m_pages["FreeMode"].MenuClicked += (id, item) =>
			{
				if (id == "vsai")
				{
					CurrentPage = null;
					// detach menu ui
					Root.Dispatcher = null;

					GameStartupParameters[] param = new GameStartupParameters[2];
                    var cardDb = GameApp.Service<CardDatabase>();

                    Deck deck1 = new Deck("MikoUnion");
                    deck1.Add(cardDb.GetModel("alice"));
                    deck1.Add(cardDb.GetModel("alice"));
                    deck1.Add(cardDb.GetModel("alice"));
                    deck1.Add(cardDb.GetModel("youmu"));
                    deck1.Add(cardDb.GetModel("youmu"));
                    deck1.Add(cardDb.GetModel("youmu"));
                    deck1.Add(cardDb.GetModel("chen"));
                    deck1.Add(cardDb.GetModel("chen"));
                    deck1.Add(cardDb.GetModel("chen"));
                    deck1.Add(cardDb.GetModel("suika"));
                    deck1.Add(cardDb.GetModel("suika"));
                    deck1.Add(cardDb.GetModel("suika"));
                    deck1.Add(cardDb.GetModel("mokou"));
                    deck1.Add(cardDb.GetModel("mokou"));
                    deck1.Add(cardDb.GetModel("mokou"));
                    deck1.Add(cardDb.GetModel("sunny"));
                    deck1.Add(cardDb.GetModel("sunny"));
                    deck1.Add(cardDb.GetModel("sunny"));



                    Deck deck2 = new Deck("MikoUnion");
                    deck2.Add(cardDb.GetModel("sakuya"));
                    deck2.Add(cardDb.GetModel("sakuya"));
                    deck2.Add(cardDb.GetModel("sakuya"));
                    deck2.Add(cardDb.GetModel("eirin"));
                    deck2.Add(cardDb.GetModel("eirin"));
                    deck2.Add(cardDb.GetModel("eirin"));
                    deck2.Add(cardDb.GetModel("aya"));
                    deck2.Add(cardDb.GetModel("aya"));
                    deck2.Add(cardDb.GetModel("aya"));
                    deck2.Add(cardDb.GetModel("suika"));
                    deck2.Add(cardDb.GetModel("suika"));
                    deck2.Add(cardDb.GetModel("suika"));
                    deck2.Add(cardDb.GetModel("komachi"));
                    deck2.Add(cardDb.GetModel("komachi"));
                    deck2.Add(cardDb.GetModel("komachi"));
                    deck2.Add(cardDb.GetModel("lunar"));
                    deck2.Add(cardDb.GetModel("lunar"));
                    deck2.Add(cardDb.GetModel("lunar"));

					param[0] = new GameStartupParameters()
					{
						m_profile = new Profile() { Name = "夜空" },
						m_deck = deck1,
                        m_hero = cardDb.GetModel("marisa"),
						m_controller = new XnaUIController()
					};
					param[0].m_profile.Decks.Add(deck1);

					param[1] = new GameStartupParameters()
					{
						m_profile = new Profile() { Name = "星奈" },
						m_deck = deck2,
                        m_hero = cardDb.GetModel("reimu"),
						m_controller = new XnaUIController()
					};
					param[1].m_profile.Decks.Add(deck2);

					GameApp.Service<GameManager>().StartGame(param);
				}
				else if (id == "back")
				{
					CurrentPage = m_pages["MainMenu"];
				}
			};

			m_pages["Quit"].MenuClicked += (id, item) =>
			{
				if (id == "quit")
				{
					GameApp.Instance.Exit();
				}
				else if (id == "back")
				{
					CurrentPage = m_pages["MainMenu"];
				}
			};

			CurrentPage = m_pages["MainMenu"];
		}

		private void LoadPage(string id)
		{
			var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle(id));
			pageStyle.Initialize();
			m_pages.Add(id, new MenuPage(pageStyle.TypedTarget));
		}

		public override void Update(float deltaTime)
		{
			foreach (var page in m_pages.Values)
			{
				if (page.Page.Dispatcher != null)
				{
					page.Page.Style.Apply();
					page.Update(deltaTime);
				}
			}
		}
	}
}
