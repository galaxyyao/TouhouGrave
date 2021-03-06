﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
    class Button : MouseTrackedControl, IRenderable
    {
        private Graphics.TexturedQuad m_normalFace;
        private Graphics.TexturedQuad m_hoverFace;
        private Graphics.TexturedQuad m_pressedFace;
        private Graphics.TexturedQuad m_disabledFace;

        public Graphics.TexturedQuad NormalFace
        {
            get { return m_normalFace; }
            set
            {
                if (value != null && value.Texture == null)
                {
                    throw new ArgumentException("Texture can't be null.");
                }

                if (value != null)
                {
                    Region = new Rectangle(0, 0, value.Texture.Width, value.Texture.Height);
                }

                m_normalFace = value;
            }
        }

        public Graphics.TexturedQuad HoverFace
        {
            get { return m_hoverFace; }
            set
            {
                if (value != null && value.Texture == null)
                {
                    throw new ArgumentException("Texture can't be null.");
                }

                m_hoverFace = value;
            }
        }

        public Graphics.TexturedQuad PressedFace
        {
            get { return m_pressedFace; }
            set
            {
                if (value != null && value.Texture == null)
                {
                    throw new ArgumentException("Texture can't be null.");
                }

                m_pressedFace = value;
            }
        }

        public Graphics.TexturedQuad DisabledFace
        {
            get { return m_disabledFace; }
            set
            {
                if (value != null && value.Texture == null)
                {
                    throw new ArgumentException("Texture can't be null.");
                }

                m_disabledFace = value;
            }
        }

        public Graphics.TextRenderer.IFormattedText ButtonText
        {
            get; set;
        }

        public XnaColor TextColor
        {
            get; set;
        }

        public XnaColor TextOutlineColor
        {
            get; set;
        }

        public Size Size
        {
            get { return Region.Size; }
        }

        public Button()
        {
            m_renderableProxy = new RenderableProxy(this);
            TextColor = XnaColor.White;
            TextOutlineColor = XnaColor.Transparent;
        }

        #region IRenderable interface

        private RenderableProxy m_renderableProxy;

        public void OnRender(RenderEventArgs e)
        {
            var transform = TransformToGlobal;

            Graphics.TexturedQuad face;
            if (!Enabled)
            {
                face = DisabledFace ?? NormalFace;
            }
            else if (IsMouseButton1Pressed)
            {
                face = PressedFace ?? HoverFace ?? NormalFace;
            }
            else if (IsMouseOver)
            {
                face = HoverFace ?? NormalFace;
            }
            else
            {
                face = NormalFace;
            }

            if (face != null)
            {
                e.RenderManager.Draw(face, Region, transform);
            }

            if (ButtonText != null)
            {
                Point position = Region.LeftTop + (Region.Size - ButtonText.Size) / 2.0f;
                var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = TextColor.ToVector4();
                drawOptions.OutlineColor = TextOutlineColor.ToVector4();
                drawOptions.Offset = new Point((int)position.X, (int)position.Y);
                e.TextRenderer.DrawText(ButtonText, transform, drawOptions);
            }
        }

        #endregion
    }
}
