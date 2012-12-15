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
            Root = new UI.EventDispatcher();
        }

        public override void Update(float deltaTime)
        {
            var xnaMouseState = Mouse.GetState();
            bool btn1Pressed = xnaMouseState.LeftButton == ButtonState.Pressed;
            bool btn2Pressed = xnaMouseState.RightButton == ButtonState.Pressed;
            var currentState = new UI.MouseState(xnaMouseState.X, xnaMouseState.Y, btn1Pressed, btn2Pressed);

            float clipSpaceX = currentState.Location.X / GameApp.Instance.GraphicsDevice.Viewport.Width * 2.0f - 1.0f;
            float clipSpaceY = 1.0f - currentState.Location.Y / GameApp.Instance.GraphicsDevice.Viewport.Height * 2.0f;
            Ray mouseRay = new Ray
            {
                Origin = new Vector3(clipSpaceX, clipSpaceY, 0.0f),
                Direction = Vector3.UnitZ
            };

            Root.RaiseEvent(new UI.MouseMoveEventArgs(currentState, mouseRay));

            if (m_lastMouseState.Button1Pressed != currentState.Button1Pressed)
            {
                Root.RaiseEvent(new UI.MouseButton1EventArgs(currentState, mouseRay));
            }
            if (m_lastMouseState.Button2Pressed != currentState.Button2Pressed)
            {
                Root.RaiseEvent(new UI.MouseButton2EventArgs(currentState, mouseRay));
            }

            m_lastMouseState = currentState;
        }

        public override void Render()
        {
            Root.RaiseEvent(new UI.RenderEventArgs());
        }
    }
}
