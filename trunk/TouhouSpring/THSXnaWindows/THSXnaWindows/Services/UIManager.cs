using System;
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
        private UI.KeyboardState m_lastKeyboardState = new UI.KeyboardState();

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

            var currentKeyboardState = new UI.KeyboardState(GameApp.Instance.KeyboardState);
            bool[] pressedKeys, releasedKeys;
            UI.KeyboardState.Differentiate(m_lastKeyboardState, currentKeyboardState, out pressedKeys, out releasedKeys);
            if (pressedKeys.Contains(true))
            {
                Root.RaiseEvent(new UI.KeyPressedEventArgs(currentKeyboardState, pressedKeys));
            }
            if (releasedKeys.Contains(true))
            {
                Root.RaiseEvent(new UI.KeyReleasedEventArgs(currentKeyboardState, releasedKeys));
            }
            m_lastKeyboardState = currentKeyboardState;

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
    }
}
