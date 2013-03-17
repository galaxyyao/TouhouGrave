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
    partial class NetworkUI : GameService
    {
        public UI.Page NetworkUIPage
        {
            get;
            private set;
        }

        public UI.TransformNode Root
        {
            get;
            private set;
        }

        private Graphics.TextRenderer.FormatOptions m_textFormatOptions;
        private UI.Label m_lblMessage;
        private UI.TextBox m_txtUserName;
        private UI.TextBox m_txtPassword;
        private THSNetwork.Client m_networkClient = null;
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
            NetworkUIPage = pageStyle.TypedTarget;
            TextRenderer.FontDescriptor m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            m_textFormatOptions = new Graphics.TextRenderer.FormatOptions(m_msgFont);
        }

        public void Enter()
        {
            NetworkUIPage.Dispatcher = Root;
            InitializeLoginPage();
        }

        private void InitializeLoginPage()
        {
            var curve = GameApp.Service<ResourceManager>().Acquire<Curve>("Curves/CardMove");

            m_txtUserName = new UI.TextBox(100, 30, m_textFormatOptions)
            {
                ForeColor = Color.Red,
                SelectionBackColor = new Color(255, 0, 0, 0.75f),
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(462, 200),
                Dispatcher = Root
            };

            m_txtPassword = new UI.TextBox(100, 30, m_textFormatOptions)
            {
                ForeColor = Color.Red,
                SelectionBackColor = new Color(255, 0, 0, 0.75f),
                PasswordChar = '*',
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(462, 250),
                Dispatcher = Root,
            };

            TextRenderer.IFormattedText textUserName = GameApp.Service<TextRenderer>().FormatText("用户名： ", m_textFormatOptions);
            var lblUserName = new UI.Label()
            {
                TextColor = Color.Red,
                Dispatcher = Root,
                FormattedText = textUserName,
                Transform = MatrixHelper.Translate(350, 200)
            };
            TextRenderer.IFormattedText textPassword = GameApp.Service<TextRenderer>().FormatText("密码： ", m_textFormatOptions);
            var lblPassword = new UI.Label()
            {
                TextColor = Color.Red,
                Dispatcher = Root,
                FormattedText = textPassword,
                Transform = MatrixHelper.Translate(350, 250)
            };
            TextRenderer.IFormattedText textMessage = GameApp.Service<TextRenderer>().FormatText(string.Empty, m_textFormatOptions);
            m_lblMessage = new UI.Label()
            {
                TextColor = Color.Red,
                Dispatcher = Root,
                FormattedText = textMessage,
                Transform = MatrixHelper.Translate(350, 300)
            };

            var resourceMgr = GameApp.Service<ResourceManager>();
            var buttonTexture = resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Button");
            Graphics.TexturedQuad m_buttonFace = new Graphics.TexturedQuad(buttonTexture);

            TextRenderer.IFormattedText textLoginButton = GameApp.Service<TextRenderer>().FormatText("登录", m_textFormatOptions);
            var btnLogin = new UI.Button()
            {
                Dispatcher = Root,
                ButtonText = textLoginButton,
                TextColor = Color.White,
                Transform = MatrixHelper.Translate(300, 350),
                NormalFace = m_buttonFace
            };
            btnLogin.MouseButton1Down += new EventHandler<UI.MouseEventArgs>(btnLogin_MouseButton1Down);

            TextRenderer.IFormattedText textReturnButton = GameApp.Service<TextRenderer>().FormatText("返回", m_textFormatOptions);
            var btnReturn = new UI.Button()
            {
                Dispatcher = Root,
                ButtonText = textReturnButton,
                TextColor = Color.White,
                Transform = MatrixHelper.Translate(450, 350),
                NormalFace = m_buttonFace
            };
            btnReturn.MouseButton1Down += delegate(object sender, UI.MouseEventArgs e)
            {
                NetworkUIPage = null;
                Root.Dispatcher = null;
                GameApp.Service<MenuUI>().Startup();
            };

            TextRenderer.IFormattedText textRegister = GameApp.Service<TextRenderer>().FormatText("注册", m_textFormatOptions);
            var btnRegister = new UI.Button()
            {
                Dispatcher = Root,
                ButtonText = textRegister,
                TextColor = Color.White,
                Transform = MatrixHelper.Translate(600, 350),
                NormalFace = m_buttonFace
            };
            btnRegister.MouseButton1Down += delegate(object sender, UI.MouseEventArgs e)
            {
                System.Diagnostics.Process.Start("http://touhouspring.com");
            };
        }

        void btnLogin_MouseButton1Down(object sender, UI.MouseEventArgs e)
        {
            //login
            m_lblMessage.FormattedText = GameApp.Service<TextRenderer>().FormatText("服务器连接中...", m_textFormatOptions);
            PrepareGameStartupParam();
            m_networkClient = GameApp.Service<Network>().THSClient;
            m_networkClient.Connect();
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

            switch (m_networkClient.NetworkStatus)
            {
                case THSNetwork.Client.NetworkStatusEnum.Connecting:
                    m_lblMessage.FormattedText = GameApp.Service<TextRenderer>().FormatText("服务器连接中...", m_textFormatOptions);
                    break;
                case THSNetwork.Client.NetworkStatusEnum.ResendingConnect:
                    m_lblMessage.FormattedText = GameApp.Service<TextRenderer>().FormatText("重新发送连接请求中...", m_textFormatOptions);
                    break;
                case THSNetwork.Client.NetworkStatusEnum.Connected:
                    m_lblMessage.FormattedText = GameApp.Service<TextRenderer>().FormatText("已连接成功...", m_textFormatOptions);
                    break;
                case THSNetwork.Client.NetworkStatusEnum.ConnectFailed:
                    m_lblMessage.FormattedText = GameApp.Service<TextRenderer>().FormatText("无法连接到服务器", m_textFormatOptions);
                    break;
                default:
                    break;
            }

            if (m_networkClient != null && m_networkClient.RoomStatus == THSNetwork.Client.RoomStatusEnum.Starting)
            {
                NetworkUIPage = null;
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
                                    new Agents.NetworkLocalPlayerAgent(),
                                    new Agents.NetworkRemoteAgent()
                                });
                }
                else
                {
                    GameApp.Service<GameManager>().StartGame(param
                            , new Agents.BaseAgent[] {
                                new Agents.NetworkRemoteAgent(),
                                    new Agents.NetworkLocalPlayerAgent()
                                });
                }
                m_networkClient.RoomStatus = THSNetwork.Client.RoomStatusEnum.Started;
                m_networkClient.CurrentGame = GameApp.Service<GameManager>().Game;
            }


            base.Update(deltaTime);
        }
    }
}
