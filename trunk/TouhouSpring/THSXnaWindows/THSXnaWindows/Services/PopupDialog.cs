using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBox = TouhouSpring.UI.ModalDialogs.MessageBox;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Graphics.TextRenderer))]
    class PopupDialog : GameService
    {
        // message box
        private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TextRenderer.IFormattedText[] m_buttonTexts = new Graphics.TextRenderer.IFormattedText[4];
        private Graphics.TextRenderer.FontDescriptor m_msgFont;

        private Stack<UI.ModalDialog> m_dialogStack = new Stack<UI.ModalDialog>();

        public void PushEmpty()
        {
            PushEmpty(UI.ModalDialog.DefaultOpacity);
        }

        public void PushEmpty(float opacity)
        {
            var modalDialog = new UI.ModalDialog
            {
                Opacity = opacity
            };

            modalDialog.Begin(GameApp.Service<UIManager>().Root.Listeners.Last(l => l is UI.EventDispatcher) as UI.EventDispatcher);
            m_dialogStack.Push(modalDialog);
        }

        public void PushMessageBox(string message)
        {
            PushMessageBox(message, MessageBox.ButtonFlags.OK, UI.ModalDialog.DefaultOpacity, null);
        }

        public void PushMessageBox(string message, Action action)
        {
            PushMessageBox(message, MessageBox.ButtonFlags.OK, UI.ModalDialog.DefaultOpacity, action != null ? (btn) => action() : (Action<int>)null);
        }

        public void PushMessageBox(string message, MessageBox.ButtonFlags buttons, Action action)
        {
            PushMessageBox(message, buttons, UI.ModalDialog.DefaultOpacity, action != null ? (btn) => action() : (Action<int>)null);
        }

        public void PushMessageBox(string message, MessageBox.ButtonFlags buttons, Action<int> action)
        {
            PushMessageBox(message, buttons, UI.ModalDialog.DefaultOpacity, action);
        }

        public void PushMessageBox(string message, MessageBox.ButtonFlags buttons, float opacity, Action<int> action)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var modalDialog = new UI.ModalDialog
            {
                Opacity = opacity
            };

            var messageBox = new MessageBox(m_buttonFace, m_buttonTexts)
            {
                Buttons = buttons,
                Text = GameApp.Service<TextRenderer>().FormatText(message, new Graphics.TextRenderer.FormatOptions(m_msgFont)),
                Dispatcher = modalDialog
            };
            messageBox.ButtonClicked += button =>
            {
                if (action != null)
                {
                    action(button);
                }
                System.Diagnostics.Debug.Assert(m_dialogStack.Peek() == modalDialog);
                PopTopDialog();
            };

            modalDialog.Begin(GameApp.Service<UIManager>().Root.Listeners.Last(l => l is UI.EventDispatcher) as UI.EventDispatcher);
            m_dialogStack.Push(modalDialog);
        }

        public void PopTopDialog()
        {
            if (m_dialogStack.Count == 0)
            {
                throw new InvalidOperationException("No modal dialog to be popped.");
            }

            m_dialogStack.Pop().End();
        }

        public override void Startup()
        {
            var device = GameApp.Instance.GraphicsDevice;
            var resourceMgr = GameApp.Service<ResourceManager>();

            m_buttonFace = new Graphics.TexturedQuad(resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Button"));

            var buttonFmtOptions = new TextRenderer.FormatOptions(new TextRenderer.FontDescriptor("Microsoft YaHei", 16));
            m_buttonTexts[MessageBox.ButtonOK] = GameApp.Service<TextRenderer>().FormatText("确定", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonCancel] = GameApp.Service<TextRenderer>().FormatText("取消", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonYes] = GameApp.Service<TextRenderer>().FormatText("是", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonNo] = GameApp.Service<TextRenderer>().FormatText("否", buttonFmtOptions);

            m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 32);
        }

        public override void Shutdown()
        {
            GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
        }
    }
}
