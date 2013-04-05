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
    partial class NetworkLoginUI : GameService
    {
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

            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle("NetworkLogin"));
            pageStyle.Initialize();
            TextRenderer.FontDescriptor m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            m_textFormatOptions = new Graphics.TextRenderer.FormatOptions(m_msgFont);
        }

        public void Enter()
        {
            InitializeLoginPage();
        }

        private void InitializeLoginPage()
        {
            var curve = GameApp.Service<ResourceManager>().Acquire<Curve>("Curves/CardMove");
            var candFmtOptions = new Graphics.TextRenderer.FormatOptions(new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 11));

            m_txtUserName = new UI.TextBox(100, 30, m_textFormatOptions, candFmtOptions)
            {
                ForeColor = Color.Red,
                SelectionBackColor = new Color(255, 0, 0, 0.75f),
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(462, 200),
                Dispatcher = Root
            };

            m_txtPassword = new UI.TextBox(100, 30, m_textFormatOptions, candFmtOptions)
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

            m_networkClient = GameApp.Service<Network>().THSClient;
            m_networkClient.Connect();
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

            if (m_networkClient != null && m_networkClient.NetworkStatus == THSNetwork.Client.NetworkStatusEnum.Connected)
            {
                Root.Dispatcher = null;
                // detach network login ui
                GameApp.Service<NetworkUI>().Enter();
            }

            base.Update(deltaTime);
        }
    }
}
