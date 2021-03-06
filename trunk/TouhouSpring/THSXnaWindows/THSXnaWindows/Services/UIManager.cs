﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TouhouSpring.Services
{
    [RenderDependency(typeof(Graphics.Scene))]
    class UIManager : GameService
    {
        private class KeyboardInputRoot : UI.EventDispatcher, UI.IFocusGroup
        {
            public UI.KeyboardInputManager KeyboardInputManager
            {
                get; private set;
            }

            public KeyboardInputRoot(Ime.ImeContext imeContext)
            {
                KeyboardInputManager = new UI.KeyboardInputManager(imeContext);
            }
        }

        private Ime.ImeContext m_imeContext;
        private UI.MouseState m_lastMouseState = new UI.MouseState(0, 0, false, false);

        public UI.EventDispatcher Root
        {
            get; private set;
        }

        public float ViewportWidth
        {
            get { return GameApp.Instance.GraphicsDevice.Viewport.Width; }
        }

        public float ViewportHeight
        {
            get { return GameApp.Instance.GraphicsDevice.Viewport.Height; }
        }

        public override void Startup()
        {
            m_imeContext = new Ime.ImeContext(GameApp.Instance.Window.Handle);
            // Ime.ImeContext provides windows messages
            m_imeContext.OnKeyDown += new Ime.KeyMessageHandler(WindowsMessage_OnKeyDown);
            m_imeContext.OnKeyUp += new Ime.KeyMessageHandler(WindowsMessage_OnKeyUp);
            Root = new KeyboardInputRoot(m_imeContext);
        }

        public override void Shutdown()
        {
            m_imeContext.Dispose();
        }

        public override void Update(float deltaTime)
        {
            if (!GameApp.Instance.IsActive)
            {
                return;
            }

            // put the KeyboardInputManager to tail
            (Root as KeyboardInputRoot).KeyboardInputManager.Dispatcher = null;
            (Root as KeyboardInputRoot).KeyboardInputManager.Dispatcher = Root;

            var xnaMouseState = GameApp.Instance.MouseState;
            bool btn1Pressed = xnaMouseState.LeftButton == ButtonState.Pressed;
            bool btn2Pressed = xnaMouseState.RightButton == ButtonState.Pressed;
            var currentMouseState = new UI.MouseState(xnaMouseState.X, xnaMouseState.Y, btn1Pressed, btn2Pressed);

            float clipSpaceX = currentMouseState.Location.X / GameApp.Instance.GraphicsDevice.Viewport.Width * 2.0f - 1.0f;
            float clipSpaceY = 1.0f - currentMouseState.Location.Y / GameApp.Instance.GraphicsDevice.Viewport.Height * 2.0f;
            Ray mouseRay = new Ray
            {
                Origin = new Vector3(clipSpaceX, clipSpaceY, 0.0f),
                Direction = Vector3.UnitZ
            };

            Root.RaiseEvent(new UI.MouseMoveEventArgs(currentMouseState, mouseRay));

            if (m_lastMouseState.Button1Pressed != currentMouseState.Button1Pressed)
            {
                Root.RaiseEvent(new UI.MouseButton1EventArgs(currentMouseState, mouseRay));
            }
            if (m_lastMouseState.Button2Pressed != currentMouseState.Button2Pressed)
            {
                Root.RaiseEvent(new UI.MouseButton2EventArgs(currentMouseState, mouseRay));
            }

            m_lastMouseState = currentMouseState;
        }

        public override void Render()
        {
            Root.RaiseEvent(new UI.RenderEventArgs());
        }

        private void WindowsMessage_OnKeyDown(char code)
        {
            Root.RaiseEvent(new UI.KeyPressedEventArgs(new UI.KeyboardState(GameApp.Instance.KeyboardState), code));
        }

        private void WindowsMessage_OnKeyUp(char code)
        {
            Root.RaiseEvent(new UI.KeyReleasedEventArgs(new UI.KeyboardState(GameApp.Instance.KeyboardState), code));
        }
    }
}
