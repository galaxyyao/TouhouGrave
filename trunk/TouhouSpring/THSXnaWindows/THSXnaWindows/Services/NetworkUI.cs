using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;
using THSNetwork = TouhouSpring.Network;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Styler))]
    [LifetimeDependency(typeof(UIManager))]
    [UpdateDependency(typeof(UIManager))]
    [LifetimeDependency(typeof(Graphics.SwfRenderer))]
    [LifetimeDependency(typeof(Network))]
    partial class NetworkUI:GameService
    {

        public UI.TransformNode Root
        {
            get;
            private set;
        }

        private Graphics.TextRenderer.FormatOptions m_textFormatOptions;
        private THSNetwork.Client m_networkClient;
        GameStartupParameters[] param = new GameStartupParameters[2];

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

            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle("Network"));
            pageStyle.Initialize();
            TextRenderer.FontDescriptor m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            m_textFormatOptions = new Graphics.TextRenderer.FormatOptions(m_msgFont);
        }

        public void Enter()
        {
            m_networkClient = GameApp.Service<Network>().THSClient;
            InitializePage();
            PrepareGameStartupParam();
        }

        private void InitializePage()
        {
            var resourceMgr = GameApp.Service<ResourceManager>();
            var buttonTexture = resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Button");
            Graphics.TexturedQuad m_buttonFace = new Graphics.TexturedQuad(buttonTexture);

            TextRenderer.IFormattedText textLoginButton = GameApp.Service<TextRenderer>().FormatText("随机对战", m_textFormatOptions);
            var btnLogin = new UI.Button()
            {
                Dispatcher = Root,
                ButtonText = textLoginButton,
                TextColor = Color.White,
                Transform = MatrixHelper.Translate(300, 350),
                NormalFace = m_buttonFace
            };
            btnLogin.MouseButton1Down += new EventHandler<UI.MouseEventArgs>(btnLogin_MouseButton1Down);
        }

        void btnLogin_MouseButton1Down(object sender, UI.MouseEventArgs e)
        {
            m_networkClient.SendMessage("startrandomgame");
        }

        public override void Update(float deltaTime)
        {
            if (m_networkClient == null)
            {
                if (GameApp.Service<Network>().THSClient == null)
                {
                    throw new Exception("Network Service is not started correctly. Network Client is null.");
                }
                m_networkClient = GameApp.Service<Network>().THSClient;
            }

            if (m_networkClient != null && m_networkClient.RoomStatus == THSNetwork.Client.RoomStatusEnum.Starting)
            {
                Root.Dispatcher = null;
                // detach menu ui

                if (m_networkClient.Seed != -1)
                {
                    System.Diagnostics.Debug.Print(string.Format("Seed: {0}", m_networkClient.Seed));
                    foreach (var playerParam in param)
                    {
                        playerParam.m_seed = m_networkClient.Seed;
                    }
                }

                if (m_networkClient.StartupIndex == 0)
                {
                    GameApp.Service<GameManager>().StartGame(param
                            , new Agents.BaseAgent[] {
                                    new Agents.LocalPlayerAgent(0),
                                    new Agents.RemoteAgent(1)
                                });
                }
                else
                {
                    GameApp.Service<GameManager>().StartGame(param
                            , new Agents.BaseAgent[] {
                                new Agents.RemoteAgent(0),
                                    new Agents.LocalPlayerAgent(1)
                                });
                }
                m_networkClient.RoomStatus = THSNetwork.Client.RoomStatusEnum.Started;
            }


            base.Update(deltaTime);
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
    }
}
