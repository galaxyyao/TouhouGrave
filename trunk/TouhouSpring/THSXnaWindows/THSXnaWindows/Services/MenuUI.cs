using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using THSNetwork = TouhouSpring.Network;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(CardDatabase))]
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Styler))]
    [LifetimeDependency(typeof(UIManager))]
    [UpdateDependency(typeof(UIManager))]
    [LifetimeDependency(typeof(Graphics.SwfRenderer))]
    partial class MenuUI : GameService
    {
        private Dictionary<string, MenuPage> m_pages = new Dictionary<string, MenuPage>();

        GameStartupParameters[] param = new GameStartupParameters[2];

        public UI.TransformNode Root
        {
            get;
            private set;
        }

        private Graphics.SwfInstance m_testAnimation;
        private Graphics.SwfInstance m_testAnimation2;

        public override void Startup()
        {
            #region Initialize Page
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
            LoadPage("Network");
            LoadPage("Quit");

            #endregion

            #region Main Menu
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
            #endregion

            #region FreeMode Menu

            PrepareGameStartupParam();

            m_pages["FreeMode"].MenuClicked += (id, item) =>
            {
                if (id == "vsai" || id == "hotseat")
                {
                    CurrentPage = null;
                    // detach menu ui
                    Root.Dispatcher = null;

                    switch (id)
                    {
                        case "vsai":
                            GameApp.Service<GameManager>().StartGame(param
                                , new Agents.BaseAgent[] {
                                    new Agents.LocalPlayerAgent(0),
                                    new Agents.AIAgent(1)
                                });
                            break;
                        case "hotseat":
                            GameApp.Service<GameManager>().StartGame(param
                                , new Agents.BaseAgent[] {
                                    new Agents.LocalPlayerAgent(0),
                                    new Agents.LocalPlayerAgent(1)
                                });
                            break;
                        default:
                            throw new InvalidOperationException("Invalid menu item");
                    }
                }
                else if (id == "vsnetwork")
                {
                    //CurrentPage = m_pages["Network"];

                    CurrentPage = null;
                    Root.Dispatcher = null;
                    GameApp.Service<GameManager>().EnterNetworkUI();
                }
                else if (id == "back")
                {
                    CurrentPage = m_pages["MainMenu"];
                }
            };
            #endregion

            #region Network Menu
            m_pages["Network"].MenuClicked += (id, item) =>
            {
                if (id == "backtofreemode")
                {
                    //CurrentPage = m_pages["FreeMode"];
                }
            };
            #endregion

            #region Quit Menu
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
            #endregion

            CurrentPage = m_pages["MainMenu"];

            var curve = GameApp.Service<ResourceManager>().Acquire<Curve>("Curves/CardMove");
            var font = new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            var fmtOptions = new Graphics.TextRenderer.FormatOptions(font);

            var txtBox = new UI.TextBox(100, 30, fmtOptions)
            {
                ForeColor = Color.Black,
                SelectionBackColor = new Color(0, 0, 0, 0.75f),
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(50, 200),
                Dispatcher = Root
            };

            //m_testAnimation = new Graphics.SwfInstance("germs")
            //{
            //    IsPlaying = true,
            //    TimeFactor = 2.0f,
            //    Transform = MatrixHelper.RotateZ(MathHelper.Pi / 4) * MatrixHelper.Translate(256f, 256f)
            //};

            //m_testAnimation2 = new Graphics.SwfInstance("28835")
            //{
            //    IsPlaying = true,
            //    Transform = MatrixHelper.Translate(768f, 0f)
            //};
        }

        private void PrepareGameStartupParam()
        {
            var cardDb = GameApp.Service<CardDatabase>();

            Deck deck1 = new Deck("Profile1");
            deck1.Add(cardDb.GetModel("hatate"));
            deck1.Add(cardDb.GetModel("hatate"));
            deck1.Add(cardDb.GetModel("hatate"));
            deck1.Add(cardDb.GetModel("koakuma"));
            deck1.Add(cardDb.GetModel("koakuma"));
            deck1.Add(cardDb.GetModel("koakuma"));
            deck1.Add(cardDb.GetModel("mokou"));
            deck1.Add(cardDb.GetModel("mokou"));
            deck1.Add(cardDb.GetModel("mokou"));
            deck1.Add(cardDb.GetModel("remilia"));
            deck1.Add(cardDb.GetModel("remilia"));
            deck1.Add(cardDb.GetModel("remilia"));
            deck1.Add(cardDb.GetModel("yuyuko"));
            deck1.Add(cardDb.GetModel("yuyuko"));
            deck1.Add(cardDb.GetModel("yuyuko"));
            deck1.Add(cardDb.GetModel("reimu"));
            deck1.Add(cardDb.GetModel("reimu"));
            deck1.Add(cardDb.GetModel("reimu"));
            deck1.Add(cardDb.GetModel("suika"));
            deck1.Add(cardDb.GetModel("suika"));
            deck1.Add(cardDb.GetModel("suika"));
            deck1.Add(cardDb.GetModel("youmu"));
            deck1.Add(cardDb.GetModel("youmu"));
            deck1.Add(cardDb.GetModel("youmu"));
            deck1.Add(cardDb.GetModel("kaguya"));
            deck1.Add(cardDb.GetModel("kaguya"));
            deck1.Add(cardDb.GetModel("kaguya"));
            deck1.Add(cardDb.GetModel("alice_2"));
            deck1.Add(cardDb.GetModel("alice_2"));
            deck1.Add(cardDb.GetModel("alice_2"));
            deck1.Add(cardDb.GetModel("marisa"));
            deck1.Add(cardDb.GetModel("marisa"));
            deck1.Add(cardDb.GetModel("marisa"));
            deck1.Assists.Add(cardDb.GetModel("eirin"));
            deck1.Assists.Add(cardDb.GetModel("patchouli"));

            Deck deck2 = new Deck("Profile2");
            deck2.Add(cardDb.GetModel("lunar"));
            deck2.Add(cardDb.GetModel("lunar"));
            deck2.Add(cardDb.GetModel("lunar"));
            deck2.Add(cardDb.GetModel("komachi"));
            deck2.Add(cardDb.GetModel("komachi"));
            deck2.Add(cardDb.GetModel("komachi"));
            deck2.Add(cardDb.GetModel("sakuya"));
            deck2.Add(cardDb.GetModel("sakuya"));
            deck2.Add(cardDb.GetModel("sakuya"));
            deck2.Add(cardDb.GetModel("meirin"));
            deck2.Add(cardDb.GetModel("meirin"));
            deck2.Add(cardDb.GetModel("meirin"));
            deck2.Add(cardDb.GetModel("sanae"));
            deck2.Add(cardDb.GetModel("sanae"));
            deck2.Add(cardDb.GetModel("sanae"));
            deck2.Add(cardDb.GetModel("kanako"));
            deck2.Add(cardDb.GetModel("kanako"));
            deck2.Add(cardDb.GetModel("kanako"));
            deck2.Add(cardDb.GetModel("cirno"));
            deck2.Add(cardDb.GetModel("cirno"));
            deck2.Add(cardDb.GetModel("cirno"));
            deck2.Add(cardDb.GetModel("keine"));
            deck2.Add(cardDb.GetModel("keine"));
            deck2.Add(cardDb.GetModel("keine"));
            deck2.Add(cardDb.GetModel("nightbug"));
            deck2.Add(cardDb.GetModel("nightbug"));
            deck2.Add(cardDb.GetModel("nightbug"));
            //deck2.Add(cardDb.GetModel("marisa"));
            //deck2.Add(cardDb.GetModel("marisa"));
            //deck2.Add(cardDb.GetModel("marisa"));
            deck2.Add(cardDb.GetModel("alice_2"));
            deck2.Add(cardDb.GetModel("alice_2"));
            deck2.Add(cardDb.GetModel("alice_2"));
            deck2.Assists.Add(cardDb.GetModel("yakumo"));
            deck2.Assists.Add(cardDb.GetModel("tenshi"));

            param[0] = new GameStartupParameters()
            {
                m_profile = new Profile() { Name = "真凉" },
                m_deck = deck1
            };
            param[0].m_profile.Decks.Add(deck1);

            param[1] = new GameStartupParameters()
            {
                m_profile = new Profile() { Name = "爱衣" },
                m_deck = deck2
            };
            param[1].m_profile.Decks.Add(deck2);
        }

        private void LoadPage(string id)
        {
            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle(id));
            pageStyle.Initialize();
            if(!m_pages.ContainsKey(id))
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

            //m_testAnimation.Update(deltaTime);
            //m_testAnimation2.Update(deltaTime);
        }

        public override void Render()
        {
            //GameApp.Service<Graphics.SwfRenderer>().Render(m_testAnimation);
            //GameApp.Service<Graphics.SwfRenderer>().Render(m_testAnimation2);
        }
    }
}
