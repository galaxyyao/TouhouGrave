﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBox = TouhouSpring.UI.ModalDialogs.MessageBox;
using NumberSelector = TouhouSpring.UI.ModalDialogs.NumberSelector;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Graphics.TextRenderer))]
    class PopupDialog : GameService
    {
        // message box
        private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TexturedQuad m_buttonFaceDisabled;
        private TextRenderer.IFormattedText[] m_buttonTexts = new Graphics.TextRenderer.IFormattedText[MessageBox.NumButtons];
        private TextRenderer.FontDescriptor m_msgFont;

        // number selector
        private TextRenderer.IFormattedText[] m_digits = new TextRenderer.IFormattedText[10];
        private TextRenderer.IFormattedText[] m_signs = new TextRenderer.IFormattedText[2];
        private TextRenderer.IFormattedText[] m_okCancelTexts = new Graphics.TextRenderer.IFormattedText[NumberSelector.NumButtons];
        private Graphics.TexturedQuad m_upButtonFace;
        private Graphics.TexturedQuad m_downButtonFace;

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

            var buttonTexts = new TextRenderer.IFormattedText[MessageBox.NumButtons];
            for (int i = 0; i < MessageBox.NumButtons; ++i)
            {
                buttonTexts[i] = ((uint)buttons & (1U << i)) != 0 ? m_buttonTexts[i] : null;
            }
            var messageBox = new MessageBox(m_buttonFace, buttonTexts)
            {
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

        public void PushNumberSelector(string message, int min, int max, Action<int, int> action)
        {
            PushNumberSelector(message, min, max, UI.ModalDialog.DefaultOpacity, action);
        }

        public void PushNumberSelector(string message, int min, int max, float opacity, Action<int, int> action)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var modalDialog = new UI.ModalDialog
            {
                Opacity = opacity
            };

            var numberSelector = new NumberSelector(m_digits, m_signs, m_upButtonFace, m_downButtonFace, m_buttonFace, m_okCancelTexts)
            {
                Text = GameApp.Service<TextRenderer>().FormatText(message, new Graphics.TextRenderer.FormatOptions(m_msgFont)),
                OkCancelButtonFaceDisabled = m_buttonFaceDisabled,
                Dispatcher = modalDialog
            };
            numberSelector.SetRange(min, max);
            numberSelector.ButtonClicked += (btn, value) =>
            {
                if (action != null)
                {
                    action(btn, value);
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
            m_buttonFaceDisabled = new Graphics.TexturedQuad(resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$ButtonDisable"));

            var buttonFmtOptions = new TextRenderer.FormatOptions(new TextRenderer.FontDescriptor("Microsoft YaHei", 16));
            m_buttonTexts[MessageBox.ButtonOK] = GameApp.Service<TextRenderer>().FormatText("确定", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonCancel] = GameApp.Service<TextRenderer>().FormatText("取消", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonYes] = GameApp.Service<TextRenderer>().FormatText("是", buttonFmtOptions);
            m_buttonTexts[MessageBox.ButtonNo] = GameApp.Service<TextRenderer>().FormatText("否", buttonFmtOptions);

            var digitFmtOptions = new TextRenderer.FormatOptions(new TextRenderer.FontDescriptor("Constantia", 36));
            10.Repeat(i => m_digits[i] = GameApp.Service<TextRenderer>().FormatText(i.ToString(), digitFmtOptions));
            m_signs[0] = GameApp.Service<TextRenderer>().FormatText("+", digitFmtOptions);
            m_signs[1] = GameApp.Service<TextRenderer>().FormatText("-", digitFmtOptions);
            m_okCancelTexts[NumberSelector.ButtonOK] = GameApp.Service<TextRenderer>().FormatText("确定", buttonFmtOptions);
            m_okCancelTexts[NumberSelector.ButtonCancel] = GameApp.Service<TextRenderer>().FormatText("取消", buttonFmtOptions);
            m_upButtonFace = new Graphics.TexturedQuad(resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Up"));
            m_downButtonFace = new Graphics.TexturedQuad(resourceMgr.Acquire<Graphics.VirtualTexture>("atlas:Textures/UI/InGame/Atlas0$Down"));

            m_msgFont = new TextRenderer.FontDescriptor("Microsoft YaHei", 32);
        }

        public override void Shutdown()
        {
            GameApp.Service<ResourceManager>().Release(m_downButtonFace.Texture);
            GameApp.Service<ResourceManager>().Release(m_upButtonFace.Texture);
            GameApp.Service<ResourceManager>().Release(m_buttonFaceDisabled.Texture);
            GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
        }
    }
}