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
    partial class DeckUI : GameService
    {
        public UI.TransformNode Root
        {
            get;
            private set;
        }

        private Graphics.TextRenderer.FormatOptions m_textFormatOptions;

        private bool m_isFirstLoaded = true;

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

            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle("Deck"));
            pageStyle.Initialize();
            TextRenderer.FontDescriptor m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            m_textFormatOptions = new Graphics.TextRenderer.FormatOptions(m_msgFont);
        }

        public void Enter()
        {
            InitializeDeckPage();
        }

        private void InitializeDeckPage()
        {
            var curve = GameApp.Service<ResourceManager>().Acquire<Curve>("Curves/CardMove");
            var candFmtOptions = new Graphics.TextRenderer.FormatOptions(new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 11));

            var resourceMgr = GameApp.Service<ResourceManager>();
            var buttonTexture = resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Button");
            Graphics.TexturedQuad m_buttonFace = new Graphics.TexturedQuad(buttonTexture);
            TextRenderer.IFormattedText textReturnButton = GameApp.Service<TextRenderer>().FormatText("<<返回", m_textFormatOptions);
            var btnReturn = new UI.Button()
            {
                Dispatcher = Root,
                ButtonText = textReturnButton,
                TextColor = Color.White,
                Transform = MatrixHelper.Translate(40, 20),
                NormalFace = m_buttonFace
            };
            if (m_isFirstLoaded)
            {
                btnReturn.MouseButton1Down += delegate(object sender, UI.MouseEventArgs e)
                {
                    Root.Dispatcher = null;
                    GameApp.Service<MenuUI>().Startup();
                };
                m_isFirstLoaded = false;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
