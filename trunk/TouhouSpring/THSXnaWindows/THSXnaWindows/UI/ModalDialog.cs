﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
    class ModalDialog : TransformNode,
        IEventListener<MouseMoveEventArgs>,
        IEventListener<MouseButton1EventArgs>,
        IEventListener<MouseButton2EventArgs>,
        IRenderable
    {
        public interface IContent
        {
            EventDispatcher Dispatcher { set; }
            void OnUpdate(float deltaTime);
            void OnPreRender();
            void OnEnd();
        }

        private RenderableProxy m_renderableProxy;

        public const float DefaultOpacity = 0.75f;

        public bool HasBegun
        {
            get; private set;
        }

        public override EventDispatcher Dispatcher
        {
            get { return base.Dispatcher; }
            set { throw new InvalidOperationException("Dispatcher can't be set directly."); }
        }

        public float Opacity
        {
            get; set;
        }

        public IContent Content
        {
            get; private set;
        }

        public ModalDialog()
        {
            m_renderableProxy = new RenderableProxy(this);

            Opacity = DefaultOpacity;
        }

        public void Begin(EventDispatcher parent, IContent content)
        {
            if (HasBegun)
            {
                throw new InvalidOperationException("The modal dialog has already begun.");
            }

            // reset size
            float screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
            float screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;

            var parentTransform = parent is ITransformNode ? (parent as ITransformNode).TransformToGlobal : Matrix.Identity;
            var toScreenSpace = Matrix.Identity;
            toScreenSpace.M11 = 2 / screenWidth;
            toScreenSpace.M22 = -2 / screenHeight;
            toScreenSpace.M41 = -1;
            toScreenSpace.M42 = 1;
            Transform = toScreenSpace * parentTransform.Invert();

            base.Dispatcher = parent;

            Content = content;
            if (content != null)
            {
                content.Dispatcher = this;
            }

            HasBegun = true;
        }

        public void End()
        {
            HasBegun = false;
            Dispatcher.Listeners.Remove(this);
        }

        void IEventListener<MouseMoveEventArgs>.RaiseEvent(MouseMoveEventArgs e)
        {
            Dispatch(e);
            e.SetHandled();
        }

        void IEventListener<MouseButton1EventArgs>.RaiseEvent(MouseButton1EventArgs e)
        {
            Dispatch(e);
            e.SetHandled();
        }

        void IEventListener<MouseButton2EventArgs>.RaiseEvent(MouseButton2EventArgs e)
        {
            Dispatch(e);
            e.SetHandled();
        }

        void IRenderable.OnRender(RenderEventArgs e)
        {
            float screenWidth = e.RenderManager.Device.Viewport.Width;
            float screenHeight = e.RenderManager.Device.Viewport.Height;
            var transform = TransformToGlobal;

            // draw a black overlay
            Graphics.TexturedQuad quadOverlay = new Graphics.TexturedQuad();
            quadOverlay.ColorScale = new Vector4(0, 0, 0, Opacity);
            e.RenderManager.Draw(quadOverlay, new Point(-0.5f, -0.5f), transform);
        }
    }
}
