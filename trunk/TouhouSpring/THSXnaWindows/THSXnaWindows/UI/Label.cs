using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
    class Label : TransformNode, IRenderable
    {
        private RenderableProxy m_renderableProxy;

        public Graphics.TextRenderer.IFormattedText FormattedText
        {
            get; set;
        }

        public bool Shadowed
        {
            get; set;
        }

        public XnaColor TextColor
        {
            get; set;
        }

        public XnaColor ShadowTextColor
        {
            get; set;
        }

        public Point ShadowOffset
        {
            get; set;
        }

        public Label()
        {
            m_renderableProxy = new RenderableProxy(this);

            TextColor = XnaColor.Black;
        }

        public virtual void OnRender(RenderEventArgs e)
        {
            if (FormattedText != null)
            {
                var transform = TransformToGlobal;

                Graphics.TextRenderer.DrawOptions drawOptions;
                if (Shadowed)
                {
                    drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                    drawOptions.ForcedColor = ShadowTextColor;
                    drawOptions.Offset = ShadowOffset;
                    e.TextRenderer.DrawText(FormattedText, transform, drawOptions);
                }
                drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = TextColor.ToVector4();
                e.TextRenderer.DrawText(FormattedText, transform, drawOptions);
            }
        }
    }
}
